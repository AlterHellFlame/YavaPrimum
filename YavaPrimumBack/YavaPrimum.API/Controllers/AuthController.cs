using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthContriller : ControllerBase
    {
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private readonly IUserService _usersService;
        private readonly IAuthService _authService;
        private INotificationsService _notificationsService;
        private readonly YavaPrimumDBContext _dBContext;
        private readonly CodeVerificationService _codeVerificationService;

        public AuthContriller(IJwtProvider jwtProvider, ITasksService tasksService, IUserService usersService, IAuthService authService, INotificationsService notificationsService, YavaPrimumDBContext dBContext, CodeVerificationService codeVerificationService)
        {
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _usersService = usersService;
            _authService = authService;
            _notificationsService = notificationsService;
            _dBContext = dBContext;
            _codeVerificationService = codeVerificationService;
        }

        [HttpPost("/register")]
        public async Task<ActionResult<Guid>> Register([FromBody] UserRequestResponse request)
        {

            Guid userId = await _authService.Register(request);
            if(userId == Guid.Empty)
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Message = "Пользователь с такой почтой уже зарегистрирован"
                });
            }
            return Ok(userId);
        }

        [HttpPost("/login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginUserRequest request)
        {
            Console.WriteLine("Попытка войти в аккаунт с данными: " + request.Email + " пароль: " + request.Password);

            string token = await _authService.Login(request.Email, request.Password);


            HttpContext.Response.Cookies.Append(JwtProvider.CookiesName, token, new CookieOptions()
            {
                Secure = true, // Используйте true, если у вас HTTPS
                SameSite = SameSiteMode.Lax, // Или Strict в зависимости от ваших требований
                Expires = DateTime.UtcNow.AddHours(24)
            });

            User user = await _usersService.GetByEMail(request.Email);

            return Ok(user.Post.Name);
        }


        [HttpPost("/sendToEmail")]
        public async Task<ActionResult<bool>> SendToEmail([FromBody] StringRequest email)
        {
            if (email == null || string.IsNullOrEmpty(email.Value))
            {
                return BadRequest("Некорректные данные");
            }

            string ret = null;
            try
            {
                await _usersService.GetByEMail(email.Value);
                ret = await _notificationsService.SendMessageToEmail(email.Value, await _codeVerificationService.GenerateCode(email.Value), "Смена пароля");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
            {
                return false;
            }
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
            return await _authService.SetNewPassByEMail(email, newPass.Value);
        }

    }
}
