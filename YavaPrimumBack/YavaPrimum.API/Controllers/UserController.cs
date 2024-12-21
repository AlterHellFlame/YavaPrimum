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
        private readonly IUserService _usersService;
        private readonly YavaPrimumDBContext _dBContext;

        public UserController(IUserService usersService, YavaPrimumDBContext dBContext)
        {
            _usersService = usersService;
            _dBContext = dBContext;
        }

        [HttpPost("/register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserRequest request)
        {
            return Ok(await _usersService.Register(request));
        }

        [HttpPost("/login")]
        public async Task<ActionResult> Login([FromBody] LoginUserRequest request)
        {
            Console.WriteLine("Попытка войти в аккаунт с данными: " + request.EMail + " пароль: " + request.Password);
            string token = await _usersService.Login(request.EMail, request.Password);

            HttpContext.Response.Cookies.Append(JwtProvider.CookiesName, token, new CookieOptions()
            {
                Expires = DateTime.Now.AddMinutes(10)
            });

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> Crt()
        {
            await _dBContext.Post.AddAsync(new Post()
            {
                PostId = Guid.NewGuid(),
                Name = "HR"
            });
            await _dBContext.Post.AddAsync(new Post()
            {
                PostId = Guid.NewGuid(),
                Name = "Кадровик"
            });
            await _dBContext.Post.AddAsync(new Post()
            {
                PostId = Guid.NewGuid(),
                Name = "Водитель"
            });

            await _dBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
