using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class TasksController : ControllerBase
    {
        private ICandidateService _candidateService;
        private ICountryService _countryService;
        private IPostService _postService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private INotificationsService _notificationsService;
        private IConverterService _converterService;

        public TasksController(
            ICandidateService candidateService,
            ICountryService countryService,
            IPostService postService,
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService,
            INotificationsService notificationsService,
            IConverterService converterService)
        {
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _notificationsService = notificationsService;
            _converterService = converterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tasks>>> GetTasks()
        {
            return Ok(await _tasksService.GetAll());//.GetAllAcrhive());
        }

        [HttpGet("/get-all-tasks")]
        public async Task<ActionResult<List<TasksResponse>>> GetAllTasks()
        {

            List<TasksResponse> taskResponses = await _converterService.ConvertToFront(await _tasksService.GetAll());

            taskResponses = taskResponses.OrderBy(tr => tr.DateTime).ToList();

            return Ok(taskResponses);
        }

        [HttpGet("/get-all-archive-tasks")]
        public async Task<ActionResult<List<TasksResponse>>> GetAllArchiveTasks()
        {

            List<TasksResponse> taskResponses = await _converterService.ConvertToFront(await _tasksService.GetAll());

            taskResponses = taskResponses.OrderBy(tr => tr.DateTime).ToList();

            return Ok(taskResponses);
        }



        [HttpGet("/get-all-tasks-of-user")]
        public async Task<ActionResult<List<CandidateFullDataResponse>>> GetAllTasksOfUser()
        {
           // Console.WriteLine("get-all-tasks");
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            List<Tasks> tasks = await _tasksService.GetAllByUserId(
                await _jwtProvider.GetUserIdFromToken(token));

            List<TasksResponse> taskResponses = await _converterService.ConvertToFront(tasks);
            Console.WriteLine("tasks " + taskResponses);
            return Ok(taskResponses);
        }

        [HttpGet("/get-all-candidates-of-user")]
        public async Task<ActionResult<List<CandidateRequestResponse>>> GetAllCandidatesOfUser()
        {
           // Console.WriteLine("get-all-candidates");
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            Guid giud = await _jwtProvider.GetUserIdFromToken(token);
            List<CandidateFullDataResponse> candidates = await _candidateService.GetAllCandidatesFullData(giud);

            Console.WriteLine("tasks " + candidates);
            return Ok(candidates);
        }


        [HttpPost("/create-new-task")]
        public async Task<ActionResult<Guid>> CreateNewTask([FromBody] InterviewCreateRequest taskRequest)
        {
            Console.WriteLine("create-new-task" + taskRequest.Candidate.Surname + " " + taskRequest);
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            Candidate candidate = new Candidate()
            {
                CandidateId = new Guid(),
                Surname = taskRequest.Candidate.Surname,
                FirstName = taskRequest.Candidate.FirstName,
                Patronymic = taskRequest.Candidate.Patronymic,
                Email = taskRequest.Candidate.Email,
                Country = await _countryService.GetByName(taskRequest.Candidate.Country),
                Phone = taskRequest.Candidate.Phone,
                Post = await _postService.GetByName(taskRequest.Candidate.Post)

            };

            Tasks task = new Tasks()
            {
                TasksId = new Guid(),
                Candidate = candidate,
                DateTime = Convert.ToDateTime(taskRequest.InterviewDate),
                Status = await _tasksService.GetStatusByName("Назначено собеседование"),
                User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token))
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);
            return Ok(task.TasksId);
        }

        [HttpPut("/new-status-for-task/{taskId:guid}")]
        public async Task<ActionResult> UpdateTaskStatus(Guid taskId, [FromBody] StringRequest status)
        {     
            Console.WriteLine(status.Value);
            Tasks task = await _tasksService.GetById(taskId);
            await _tasksService.SetNewStatus(task, status.Value);


            return Ok();
        }

        [HttpPut("/set-date/{notificationId:guid}")]
        public async Task<ActionResult> SetDate(Guid notificationId, StringRequest dateTime)
        {

            Notifications notification = await _notificationsService.GetById(notificationId);

            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok();
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];


            if (notification.Task.Status.Name == "Собеседование пройдено")
            {
                Tasks newTask = new Tasks
                {
                    TasksId = Guid.NewGuid(),
                    Status = await _tasksService.GetStatusByName("Взят кандидат"),
                    Candidate = notification.Task.Candidate,
                    User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                    DateTime = Convert.ToDateTime(dateTime.Value) // Текущая дата + 7 дней
                };
                await _tasksService.Create(newTask);

                await _notificationsService.SendMessage(newTask);
                await _notificationsService.ReadNotificationOfCandidate(notification.Task.Candidate.CandidateId);

            }
            else
            {
                Tasks KadrTask = await _tasksService.GetLastActiveTask(notification.Task.Candidate.CandidateId, 
                    await _jwtProvider.GetUserIdFromToken(token));

                await _tasksService.SetNewDate(KadrTask, Convert.ToDateTime(dateTime.Value));

                await _notificationsService.SendMessage(KadrTask);
                await _notificationsService.ReadNotification(notification.NotificationsId);
            }

            Tasks newTaskForHr = new Tasks
            {
                TasksId = Guid.NewGuid(),
                Status = await _tasksService.GetStatusByName("Подтверждение даты"),
                Candidate = notification.Task.Candidate,
                User = notification.Task.User,
                DateTime = Convert.ToDateTime(dateTime.Value)
            };

            await _tasksService.Create(newTaskForHr);
            await _tasksService.SetActive(newTaskForHr.TasksId);

            return Ok();
        }

        [HttpPut("/set-time/{notificationId:guid}")]
        public async Task<ActionResult> SetTime(Guid notificationId, StringRequest dateTime)
        {

            Notifications notification = await _notificationsService.GetById(notificationId);

            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok();
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];


            Tasks KadrTask = await _tasksService.GetLastActiveTask(notification.Task.Candidate.CandidateId,
                await _jwtProvider.GetUserIdFromToken(token));

            DateTime combinedDateTime = KadrTask.DateTime.Date.Add(Convert.ToDateTime(dateTime.Value).TimeOfDay);

            await _tasksService.SetNewDate(KadrTask, combinedDateTime);

            await _notificationsService.SendMessage(KadrTask);
            await _notificationsService.ReadNotification(notification.NotificationsId); ;


            Tasks newTaskForHr = new Tasks
            {
                TasksId = Guid.NewGuid(),
                Status = await _tasksService.GetStatusByName("Подтверждение времени"),
                Candidate = notification.Task.Candidate,
                User = notification.Task.User,
                DateTime = combinedDateTime
            };

            await _tasksService.Create(newTaskForHr);
            await _tasksService.SetActive(newTaskForHr.TasksId);

            return Ok();
        }

    }
}
