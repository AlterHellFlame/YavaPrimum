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
                User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                AdditionalData = taskRequest.AdditionalData
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);
            return Ok(task.TasksId);
        }

        [HttpPut("/new-status-for-task/{taskId:guid}")]
        public async Task<ActionResult> UpdateTaskStatus(
            Guid taskId,
            [FromBody] StatusUpdateRequest request) // Изменили тип параметра
        {
            var task = await _tasksService.GetById(taskId);

            try
            {
                // Вызываем метод с новыми параметрами
                await _tasksService.SetNewStatus(
                    task,
                    request);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new
                {
                    StatusCode = 106,
                    Message = "Почты не существует"
                });
            }

            return Ok();
        }

        [HttpPut("/confirm-dateTime/{taskId:guid}")]
        public async Task<ActionResult> ConfirmDateTime(
        Guid taskId,
        [FromBody] StatusUpdateRequest request) // Изменили тип параметра
        {
            Tasks oldTask = await _tasksService.GetById(taskId);
            User hr = await _userService.GetAnotherUserOfCandidate(await _tasksService.GetById(taskId));

            Tasks task = await _tasksService.GetLastActiveTask(oldTask.Candidate.CandidateId, hr.UserId);

            await UpdateTaskStatus(task.TasksId, request);

            if(request.Status == "Дата подтверждена")
            oldTask.Status = await _tasksService.GetStatusByName("Дата была подтверждена");

            await _notificationsService.ReadNotificationOfTask(taskId);

            return Ok();
        }


        bool isNew = true; 
        [HttpPut("/set-date/{notificationId:guid}")]
        public async Task<ActionResult> SetDate(Guid notificationId, StringRequest dateTime)
        {

            Notifications notification = await _notificationsService.GetById(notificationId);

            isNew = false;
            await SetDateTask(notification.Task.TasksId, dateTime);
            await _notificationsService.ReadNotification(notification.NotificationsId); ;
            isNew = true;

            return Ok();
        }

        [HttpPut("/set-date-task/{taskId:guid}")]
        public async Task<ActionResult> SetDateTask(Guid taskId, [FromBody] StringRequest dateTime)
        {

            Tasks task = await _tasksService.GetById(taskId);

            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok();
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];


            if (task.Status.Name == "Собеседование пройдено" || task.Status.Name == "Выполнено тестовое задание")
            {
                Tasks newTask = new Tasks
                {
                    TasksId = Guid.NewGuid(),
                    Status = await _tasksService.GetStatusByName("Взят кандидат"),
                    Candidate = task.Candidate,
                    User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                    DateTime = Convert.ToDateTime(dateTime.Value),
                    AdditionalData = task.AdditionalData
                };
                await _tasksService.Create(newTask);


                await _notificationsService.ReadNotificationOfCandidate(task.Candidate.CandidateId);
                await _notificationsService.SendMessage(newTask);

            }
            else
            {
                Tasks KadrTask = await _tasksService.GetLastActiveTask(task.Candidate.CandidateId,
                    await _jwtProvider.GetUserIdFromToken(token));

                KadrTask.Status = await _tasksService.GetStatusByName("Ожидается подтверждение даты");
                await _tasksService.SetNewDate(KadrTask, Convert.ToDateTime(dateTime.Value));

                await _notificationsService.ReadAllNotificationOfCandidate(KadrTask.Candidate.CandidateId);
                if (!isNew) await _notificationsService.SendMessage(KadrTask);
                else await _notificationsService.SendMessageToChangeDataOrTime(KadrTask, true, false);

            }

            if (true)
            {
                Tasks newTaskForHr = new Tasks
                {
                    TasksId = Guid.NewGuid(),
                    Status = await _tasksService.GetStatusByName("Подтверждение даты"),
                    Candidate = task.Candidate,
                    User = !isNew? task.User : await _userService.GetAnotherUserOfCandidate(task),
                    DateTime = Convert.ToDateTime(dateTime.Value),
                    AdditionalData = task.AdditionalData
                };

                await _tasksService.Create(newTaskForHr);
                await _tasksService.SetActive(newTaskForHr.TasksId);
            }

            return Ok();
        }

        [HttpPut("/set-time/{notificationId:guid}")]
        public async Task<ActionResult> SetTime(Guid notificationId, StringRequest dateTime)
        {

            Notifications notification = await _notificationsService.GetById(notificationId);

            isNew = false;
            await SetTimeTask(notification.Task.TasksId, dateTime);
            await _notificationsService.ReadNotification(notification.NotificationsId); ;
            isNew = true;
            return Ok();
        }

        [HttpPut("/set-time-task/{taskId:guid}")]
        public async Task<ActionResult> SetTimeTask(Guid taskId, [FromBody] StringRequest dateTime)
        {

            Tasks task = await _tasksService.GetById(taskId);

            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok();
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];


            Tasks KadrTask = await _tasksService.GetLastActiveTask(task.Candidate.CandidateId,
                await _jwtProvider.GetUserIdFromToken(token));

            KadrTask.Status = await _tasksService.GetStatusByName("Ожидается подтверждение времени");

            DateTime combinedDateTime = KadrTask.DateTime.Date.Add(Convert.ToDateTime(dateTime.Value).TimeOfDay);

            await _tasksService.SetNewDate(KadrTask, combinedDateTime);

            await _notificationsService.ReadAllNotificationOfCandidate(KadrTask.Candidate.CandidateId);

            if (!isNew) await _notificationsService.SendMessage(KadrTask);
            else await _notificationsService.SendMessageToChangeDataOrTime(KadrTask, false, false);

            if (true)
            {
                Tasks newTaskForHr = new Tasks
                {
                    TasksId = Guid.NewGuid(),
                    Status = await _tasksService.GetStatusByName("Подтверждение времени"),
                    Candidate = task.Candidate,
                    User = !isNew ? task.User : await _userService.GetAnotherUserOfCandidate(task),
                    DateTime = combinedDateTime
                };

                await _tasksService.Create(newTaskForHr);
                await _tasksService.SetActive(newTaskForHr.TasksId);
            }
            return Ok();
        }

        [HttpPut("/change-dateTime-without-check/{taskId:guid}")]
        public async Task<ActionResult> ChangeDateTimeWithoutCheck(Guid taskId, [FromBody] StringRequest newDateTime)
        {

            await _tasksService.ChangeTimeWithoutCheck(taskId, Convert.ToDateTime(newDateTime.Value));

            return Ok();
        }

        [HttpPut("/change-dateTime/{taskId:guid}")]
        public async Task<ActionResult> ChangeDateTime(Guid taskId, [FromBody] ChangeDateTimeRequest request)
        {

            await _tasksService.ChangeTime(taskId, request);

            return Ok();
        }


    }
}
