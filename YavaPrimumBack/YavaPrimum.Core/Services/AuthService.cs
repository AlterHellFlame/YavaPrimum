using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace YavaPrimum.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPostService _postService;
        private readonly ICompanyService _companyService;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly INotificationsService _notificationsService;
        private readonly CodeVerificationService _codeVerificationService;

        public AuthService(YavaPrimumDBContext dbContext, IJwtProvider jwtProvider, IPostService postService, ICompanyService companyService, IConfiguration config, IUserService userService, INotificationsService notificationsService, CodeVerificationService codeVerificationService)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
            _postService = postService;
            _companyService = companyService;
            _config = config;
            _userService = userService;
            _notificationsService = notificationsService;
            _codeVerificationService = codeVerificationService;
        }

        public string GeneratePasswordHas(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public async Task<User> SetNewPassByEMail(string email, string newPass)
        {
            User user = await _userService.GetByEMail(email);
            user.PasswordHash = GeneratePasswordHas(newPass);
            _dbContext.SaveChanges();
            return user;

        }

        private string GenerateLoginCode()
        {
            int passwordLenth = 8;
            var random = new Random();
            var result = string.Join("",
             Enumerable.Range(0, passwordLenth)
             .Select(i =>
              random.Next(0, 10) % 2 == 0 ?
               (char)('a' + random.Next(26)) + "" :
               random.Next(1, 10) + "")
             );
            return result;
        }

        public async Task<Guid> Register(UserRequestResponse userRequestResponce)
        {
            // Проверяем, существует ли уже пользователь с такой почтой
            bool userExists = await _dbContext.User
                .AnyAsync(u => u.Email == userRequestResponce.Email);

            if (userExists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Пользователь с такой почтой уже зарегистрирован");
                Console.ForegroundColor = ConsoleColor.Gray;
                return Guid.Empty;
            }

            string password = GenerateLoginCode();
            string passwordHash = GeneratePasswordHas(password); //TODO Сделать рандомный пароль

            User user = new User
            {
                UserId = Guid.NewGuid(),

                FirstName = userRequestResponce.FirstName,
                Surname = userRequestResponce.Surname,
                Patronymic = userRequestResponce.Patronymic,
                Email = userRequestResponce.Email,
                Phone = userRequestResponce.Phone,

                ImgUrl = "default.jpg",
                PasswordHash = passwordHash,
                Post = await _postService.GetByName(userRequestResponce.Post),
                Company = await _companyService.GetByName(userRequestResponce.Company),
            };

            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            string message = $"Вы были зарегестрированы на сайте YavaPrimum. \nВаш пароль - {password}. " +
                $"В целях безопасности просим вас не сообщать пароль никому и поменять его при первой возможности";

            await _notificationsService.SendMessageToEmail(userRequestResponce.Email, message, "Регистрация в YavaPrimum");

            return user.UserId;
        }

        public async Task<string> Login(string email, string password)
        {
            User user = await _userService.GetByEMail(email);

            bool result = Verify(password, user.PasswordHash);

            if (result == false)
                throw new Exception("Логин или пароль введены неверно: " + email);

            string token = _jwtProvider.GenerateToken(user);

            return token;
        }

       
    }
}
