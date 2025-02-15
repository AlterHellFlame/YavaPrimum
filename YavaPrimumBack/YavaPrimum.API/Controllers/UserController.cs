using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private readonly IUserService _usersService;
        private readonly YavaPrimumDBContext _dBContext;
        private readonly CodeVerificationService _codeVerificationService;

        public UserController(IJwtProvider jwtProvider, ITasksService tasksService, 
            IUserService usersService, YavaPrimumDBContext dBContext, CodeVerificationService codeVerificationService)
        {
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _usersService = usersService;
            _dBContext = dBContext;
            _codeVerificationService = codeVerificationService;
        }

        [HttpPost("/register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
        {
            return Ok(await _usersService.Register(request));
        }

        [HttpPost("/login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
        {
            Console.WriteLine("Попытка войти в аккаунт с данными: " + request.EMail + " пароль: " + request.Password);

            string token = await _usersService.Login(request.EMail, request.Password);


            HttpContext.Response.Cookies.Append(JwtProvider.CookiesName, token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true, // Используйте true, если у вас HTTPS
                SameSite = SameSiteMode.Lax, // Или Strict в зависимости от ваших требований
                Expires = DateTime.UtcNow.AddHours(12)
            });

            User user = await _usersService.GetByEMail(request.EMail);

            return Ok(user.Post.Name);
        }

        [HttpPost("/sendToEmail")]
        public async Task<ActionResult<bool>> SendToEmail([FromBody] StringRequest email)
        {
            if (email == null || string.IsNullOrEmpty(email.Value))
            {
                return BadRequest("Некорректные данные");
            }

            string ret = await _usersService.SendMessageToEmail(email.Value, "123456", "Смена пароля");
            if (ret != null)
            {
                return true;
            }
            return false;
        }

        [HttpPost("/checkCode/{email}")]
        public async Task<ActionResult<bool>> CheckCode(string email, [FromBody] StringRequest code)
        {
            return await _codeVerificationService.VerifyCode(email, code.Value);
        }

        [HttpPost("/newPass/{email}")]
        public async Task<ActionResult<User>> NewPass(string email, [FromBody] StringRequest newPass)
        {
            return await _usersService.SetNewPassByEMail(email, newPass.Value);
        }

        [HttpGet]
        public async Task<ActionResult> Crt()
        {
            await _dBContext.Country.AddAsync(new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "Монголия"
            });
            await _dBContext.Country.AddAsync(new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "Китай"
            });
            await _dBContext.Country.AddAsync(new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "ОАЭ"
            });

            await _dBContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("/userData")]
        public async Task<ActionResult<UserResponse>> GetUserData()
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TaskResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            return await _usersService.GetByIdToFront(await _jwtProvider.GetUserIdFromToken(token));
        }
    }
}
