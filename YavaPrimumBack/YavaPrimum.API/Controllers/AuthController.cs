using Microsoft.AspNetCore.Authentication;
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
    public class AuthContriller : ControllerBase
    {
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private readonly IUserService _usersService;
        private readonly IAuthService _authService;
        private readonly YavaPrimumDBContext _dBContext;
        private readonly CodeVerificationService _codeVerificationService;

        public AuthContriller(IJwtProvider jwtProvider, ITasksService tasksService, IAuthService authService,
            IUserService usersService, YavaPrimumDBContext dBContext, CodeVerificationService codeVerificationService)
        {
            _jwtProvider = jwtProvider;
            _authService = authService;
            _tasksService = tasksService;
            _usersService = usersService;
            _dBContext = dBContext;
            _codeVerificationService = codeVerificationService;
        }

        [HttpPost("/crt")]
        public async Task<ActionResult> crt()
        {

            var a = new TasksStatus
            {
                TasksStatusId = Guid.NewGuid(),
                Name = "Подбор кадровика",
            };

            var b = new TasksStatus
            {
                TasksStatusId = Guid.NewGuid(),
                Name = "Выполнено",
            };

            _dBContext.TasksStatus.AddRange(a,b);

            // Сохранение изменений в базе данных
            await _dBContext.SaveChangesAsync(); 

            /*
            // Создание стран
            var russia = new Country { CountryId = Guid.NewGuid(), Name = "Россия" };
            var belarus = new Country { CountryId = Guid.NewGuid(), Name = "Беларусь" };
            var mongolia = new Country { CountryId = Guid.NewGuid(), Name = "Монголия" };
            var kazakhstan = new Country { CountryId = Guid.NewGuid(), Name = "Казахстан" };

            _dBContext.Country.AddRange(russia, belarus, mongolia, kazakhstan);

            // Создание должностей
            var hr = new Post { PostId = Guid.NewGuid(), Name = "HR" };
            var personnelOfficer = new Post { PostId = Guid.NewGuid(), Name = "Кадровик" };
            var driver = new Post { PostId = Guid.NewGuid(), Name = "Водитель" };
            var programmer = new Post { PostId = Guid.NewGuid(), Name = "Программист" };

            _dBContext.Post.AddRange(hr, personnelOfficer, driver, programmer);

            // Создание компаний

            var primway = new Company
            {
                CompanyId = Guid.NewGuid(),
                Name = "Примвей",
                Country = belarus
            };

            var firstElement = new Company
            {
                CompanyId = Guid.NewGuid(),
                Name = "Первый элемент",
                Country = russia
            };

            var interAutoTrans = new Company
            {
                CompanyId = Guid.NewGuid(),
                Name = "ИнтерАвтоТранс",
                Country = kazakhstan
            };

            var rusAuto = new Company
            {
                CompanyId = Guid.NewGuid(),
                Name = "Рус-Авто",
                Country = russia
            };

            _dBContext.Company.AddRange(primway, firstElement, interAutoTrans, rusAuto);

            // Сохранение изменений в базе данных
            await _dBContext.SaveChangesAsync();*/

            return Ok();
        }


        [HttpPost("/register")]
        public async Task<ActionResult<Guid>> Register([FromBody] UserRequestResponse request)
        {
            return Ok(await _authService.Register(request));
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
                Expires = DateTime.UtcNow.AddHours(12)
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

            string ret = await _authService.SendMessageToEmail(email.Value, await _codeVerificationService.GenerateCode(email.Value), "Смена пароля");
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
