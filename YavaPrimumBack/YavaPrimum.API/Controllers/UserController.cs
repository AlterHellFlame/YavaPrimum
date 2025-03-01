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
        private ICandidateService _candidateService;
        private ICountryService _countryService;
        private IPostService _postService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private INotificationsService _notificationsService;

        public UserController(
            ICandidateService candidateService,
            ICountryService countryService,
            IPostService postService,
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService,
            INotificationsService notificationsService)
        {
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _notificationsService = notificationsService;
        }


        [HttpGet("/get-user-data")]
        public async Task<ActionResult<UserRequestResponse>> GetUserData()
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            return await _userService.GetByIdToFront(await _jwtProvider.GetUserIdFromToken(token));
        }


        [HttpGet("/get-posts-countries")]
        public async Task<ActionResult<List<PostCountryResponse>>> GetPostsCountries()
        {
            List<Post> Posts = await _postService.GetAll();
            List<Country> Countries = await _countryService.GetAll();

            PostCountryResponse postCountry = new PostCountryResponse()
            {
                Posts = Posts.Select(post => post.Name).ToList(),
                Countries = Countries.Select(country => country.Name).ToList(),
            };
            return Ok(postCountry);
        }


        [HttpGet("/get-notifications")]
        public async Task<ActionResult<List<NotificationsResponse>>> GetNotifications()
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            return await _notificationsService.GetNotificatonsByUserId(await _jwtProvider.GetUserIdFromToken(token));
        }

        [HttpPost("/read-notification/{taskId:guid}")]
        public async Task<ActionResult> ReadNotification(Guid taskId)
        {
            _notificationsService.ReadNotification(taskId);
            return Ok();
        }

    }
}
