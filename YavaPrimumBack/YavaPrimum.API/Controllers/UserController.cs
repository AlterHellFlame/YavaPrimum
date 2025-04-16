using Azure.Core;
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
        private ICompanyService _companyService;
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
            INotificationsService notificationsService,
            ICompanyService companyService)
        {
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _notificationsService = notificationsService;
            _companyService = companyService;
        }


        [HttpPost("/update-user-data")]
        public async Task<ActionResult<UserRequestResponse>> UpdateUserData([FromBody] UserRequestResponse request)
        {
            User user = await _userService.GetById(request.UserId);


            user.Surname = request.Surname;
            user.FirstName = request.FirstName;
            user.Patronymic = request.Patronymic;
            user.Company = await _companyService.GetByName(request.Company);
            user.Email = request.Email;
            user.Phone = request.Phone;

            user.Post = await _postService.GetByName(request.Post);
            await _userService.UpdateUser(user);

            return Ok();
        }


        [HttpDelete("/delete-user/{userId:guid}")]
        public async Task<ActionResult<UserRequestResponse>> DeleteUser(Guid userId)
        {
            await _userService.DeleteUserById(userId);

            return Ok();
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

        [HttpGet("/get-all-users-data")]
        public async Task<ActionResult<List<UserRequestResponse>>> GetAllUsersData()
        {

            return await _userService.ConvertToFront(await _userService.GetAll());
        }


        [HttpGet("/get-posts-countries")]
        public async Task<ActionResult<List<PostCountryResponse>>> GetPostsCountries()
        {
            List<Post> Posts = await _postService.GetAll();
            List<Country> Countries = await _countryService.GetAll();
            List<Company> Companies = await _companyService.GetAll();

            PostCountryResponse postCountry = new PostCountryResponse()
            {
                Posts = Posts.Select(post => post.Name).ToList(),
                Countries = Countries.Select(country => country.Name).ToList(),
                Companies = Companies.Select(company => company.Name).ToList(),
                PhoneMask = Countries.Select(country => country.PhoneMask).ToList()
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

            return await _notificationsService.GetAllByUserId(await _jwtProvider.GetUserIdFromToken(token));
        }

        [HttpPut("/read-notification/{notificationId:guid}")]
        public async Task<ActionResult> ReadNotification(Guid notificationId)
        {

            Console.WriteLine("Сообщение прочитано");

            Notifications notification = await _notificationsService.GetById(notificationId);

            if (notification.ArchiveTasks.Status.Name == "Собеседование пройдено")
            {
                if (HttpContext.Request.Cookies.Count == 0)
                {
                    Console.WriteLine("Куков нет");
                    return Ok();
                }
                string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];


                Tasks newTask = new Tasks
                {
                    TasksId = Guid.NewGuid(),

                    Status = await _tasksService.GetStatusByName("Взят кандидат"),
                    Candidate = notification.ArchiveTasks.Task.Candidate,
                    CandidatePost = notification.ArchiveTasks.Task.CandidatePost,
                    User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                    DateTime = DateTime.Now.AddDays(7) // Текущая дата + 7 дней
                };

                await _tasksService.Create(newTask);
            }
            await _notificationsService.ReadNotification(notificationId);
            return Ok();
        }

    }
}
