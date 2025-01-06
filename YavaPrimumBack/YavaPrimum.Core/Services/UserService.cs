using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class UserService : IUserService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPostService _postService;
        private readonly ICompanyService _companyService;

        public UserService(YavaPrimumDBContext dbContext, 
            IJwtProvider jwtProvider,
            IPostService postService,
            ICompanyService companyService)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
            _postService = postService;
            _companyService = companyService;
        }
        public string GeneratePasswordHas(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public async Task<Guid> Register(RegisterUserRequest registerUserRequest)
        {
            //string passwordHash = GeneratePasswordHas(registerUserRequest.Password);
            string passwordHash = GeneratePasswordHas("Aboba");

            UserRegisterInfo userRegisterInfo = new UserRegisterInfo
            {
                Email = registerUserRequest.Email,
                PasswordHash = passwordHash
            };

            User user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = registerUserRequest.FirstName,
                SecondName = registerUserRequest.SecondName,
                SurName = registerUserRequest.SurName,
                UserRegisterInfo = userRegisterInfo,
                Post = await _postService.GetByName(registerUserRequest.Post),
                Company = await _companyService.GetByName(registerUserRequest.Company),
            };
            await _dbContext.UserRegisterInfo.AddAsync(userRegisterInfo);
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user.UserId;
        }

        public async Task<string> Login(string email, string password)
        {
            User user = await GetByEMail(email);


            bool result = Verify(password, user.UserRegisterInfo.PasswordHash);

            if (result == false)
                throw new Exception("Логин или пароль введены неверно: " + email);

            string token = _jwtProvider.GenerateToken(user);

            return token;
        }

        public async Task<User> GetByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserRegisterInfo)
                .Include(p => p.Post)
                .FirstOrDefaultAsync(
                u => u.UserRegisterInfo.Email == email);

            if (user == null)
                throw new ArgumentNullException("No user with that email");

            return user;
        }

        public async Task<User> GetById(Guid id)
        {
            return await _dbContext.User
                .Include(u => u.UserRegisterInfo)
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.User
                .Include(p=> p.Post)
                .Include (u => u.UserRegisterInfo)
                .Include (c => c.Company)
                .Include(co => co.Company.Country)
                .ToListAsync();
        }

        public async Task<UserResponse> GetByIdToFront(Guid id)
        {
            User? user = await _dbContext.User
                .Include(u => u.UserRegisterInfo)
                .Include(p => p.Post)
                .Include(c => c.Company.Country)
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new ArgumentNullException("No user with that id");

            UserResponse userResponse = new UserResponse()
            {
                SecondName = user.SecondName,
                FirstName = user.FirstName,
                SurName = user.SurName,
                Post = user.Post.Name,
                Company = user.Company.Name,
                Country = user.Company.Country.Name,
                Email = user.UserRegisterInfo.Email,
                Phone = user.Phone,
                ImgUrl = user.ImgUrl
            };

            return userResponse;
        }
    }
}
