using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using System.Net.Http;

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
        private readonly CodeVerificationService _codeVerificationService;

        public AuthService(YavaPrimumDBContext dbContext,
            IConfiguration config,
            IJwtProvider jwtProvider,
            IPostService postService,
            ICompanyService companyService,
            IUserService userService,
            CodeVerificationService codeVerificationService)
        {
            _config = config;
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
            _postService = postService;
            _companyService = companyService;
            _userService = userService;
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

        public async Task<Guid> Register(UserRequestResponse userRequestResponce)
        {
            string password = "Aboba";
            string passwordHash = GeneratePasswordHas(password);//TODO Сделать рандомный пароль

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
            await SendMessageToEmail(userRequestResponce.Email, message, "Регистрация");

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

        public async Task<string> SendMessageToEmail(string email, string myMessage, string subject)
        {
            if (!await _userService.IsUserExistByEMail(email)) return null;

            MailAddress mailFrom = new MailAddress("yavaprimum@mail.ru", "YavaPrimum");
            MailAddress mailTo = new MailAddress(email);
            MailMessage message = new MailMessage(mailFrom, mailTo);
            message.Body = myMessage;
            //message.Body = $"Ваш код доступа: {await _codeVerificationService.GenerateCode(email)}. Не сообщайте его никому ";
            message.Subject = subject;

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = "smtp.mail.ru",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailFrom.Address, "vuy7pBNKJC1LEESaksGN")
            };
            try
            {
                smtpClient.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine(message);
            return email;
        }
    }
}
