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
    public class TasksController : ControllerBase
    {
        private ICandidateService _candidateService;
        private ICountryService _countryService;
        private IPostService _postService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private INotificationsService _notificationsService;

        public TasksController(
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

        [HttpGet]
        public async Task<ActionResult<List<Tasks>>> GetTasks()
        {
            return Ok(await _tasksService.GetAllAcrhive());
        }

        [HttpGet("/get-all-tasks")]
        public async Task<ActionResult<List<TasksResponse>>> GetAllTasks()
        {

            List<TasksResponse> taskResponses = await _tasksService.ConvertToFront(await _tasksService.GetAll());

            taskResponses = taskResponses.OrderBy(tr => tr.DateTime).ToList();

            return Ok(taskResponses);
        }

        [HttpGet("/get-all-archive-tasks")]
        public async Task<ActionResult<List<TasksResponse>>> GetAllArchiveTasks()
        {

            List<TasksResponse> taskResponses = await _tasksService.ConvertArchiveToFront(await _tasksService.GetAllAcrhive());

            taskResponses = taskResponses.OrderBy(tr => tr.DateTime).ToList();

            return Ok(taskResponses);
        }



        [HttpGet("/get-all-tasks-of-user")]
        public async Task<ActionResult<List<TasksResponse>>> GetAllTasksOfUser()
        {
            Console.WriteLine("get-all-tasks");
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            List<Tasks> tasks = await _tasksService.GetAllByUserId(
                await _jwtProvider.GetUserIdFromToken(token));

            List<TasksResponse> taskResponses = await _tasksService.ConvertToFront(tasks);
            Console.WriteLine("tasks " + taskResponses);
            return Ok(taskResponses);
        }

        [HttpGet("/get-all-candidates-of-user")]
        public async Task<ActionResult<List<CandidateRequestResponse>>> GetAllCandidatesOfUser()
        {
            Console.WriteLine("get-all-candidates");
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            List<CandidateRequestResponse> candidates = await _candidateService.GetAll();

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
                Phone = taskRequest.Candidate.Phone
            };

            Tasks task = new Tasks()
            {
                TasksId = new Guid(),
                Candidate = candidate,
                DateTime = Convert.ToDateTime(taskRequest.InterviewDate),
                Status = await _tasksService.GetStatusByName("Назначено собеседование"),
                User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                CandidatePost = await _postService.GetByName(taskRequest.Post)
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);
            return Ok(task.TasksId);
        }

        [HttpPut("/new-status-for-task/{taskId:guid}")]
        public async Task<ActionResult> UpdateTaskStatus(Guid taskId, [FromBody] StringRequest status)
        {
          
            // Получаем задачу по идентификатору
            Console.WriteLine(status);
            Tasks task = await _tasksService.GetById(taskId);
            await _tasksService.SetNewStatus(task, status.Value);

            return Ok();
        }

        /*[HttpPut("/give-candidate-to-po/{notificationId:guid}")]
        public async Task<ActionResult> GiveCandidateToPo(Guid notificationId, [FromBody] StringRequest status)
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok();
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            Notifications notification = await _notificationsService.GetById(notificationId);


            Tasks newTask = new Tasks
            {
                TasksId = Guid.NewGuid(),

                Status = await _tasksService.GetStatusByName("Взят кандидат"),
                Candidate = notification.ArchiveTasks.Task.Candidate,
                CandidatePost = notification.ArchiveTasks.Task.CandidatePost,
                User = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(token)),
                DateTime = DateTime.Now.AddDays(7) // Текущая дата + 7 дней
            };*/

            // Получаем задачу по идентификатору
            /*Tasks task = await _tasksService.GetById(taskId);
            await _tasksService.SetNewStatus(task, status.Value);*/

            /*if (status.Value == "Подбор кадровика")
            {
                Console.WriteLine("Рассылка для поиска кадровика");
                
                await _tasksService.SetNewStatus(task, "Выполнено");

                // Создаем новый идентификатор для пользователя
                Guid userId = new Guid("7fc78775-1e04-4b47-bb62-516d99a6f0e4");

                // Создаем новую задачу
                Tasks newTask = new Tasks
                {
                    TasksId = Guid.NewGuid(),

                    Status = await _tasksService.GetStatusByName("Задать дату"),
                    Candidate = task.Candidate,
                    User = await _userService.GetById(userId) //TODO Заменить на актуальный идентификатор пользователя
                };

                // Сохраняем новую задачу
                Guid newTaskId = await _tasksService.Create(newTask);
                Console.WriteLine(newTaskId);
            }*/
            /*
            return Ok();
        }*/




        /*[HttpPost("Interview/{taskId:guid}")]
        public async Task<ActionResult> CommitTask(Guid taskId, StringRequest status)
        {
            Console.WriteLine(status.Value);
            switch (status.Value)
            {
                case "passed":
                {
                    await _tasksService.PassedInterview(taskId);
                    break;
                };
                case "faild":
                {
                    await _tasksService.FaidInterview(taskId);
                        break;
                };
                default:
                    break;
            }
            //await _tasksService.PassedInterview(taskId);
            return Ok();
        }

        [HttpPost("RepeatInterview/{taskId:guid}")] // Добавлен слэш перед taskId
        public async Task<ActionResult> RepeatInterview(Guid taskId, [FromBody] StringRequest dateTime)
        {
            await _tasksService.RepeatInterview(taskId, dateTime.Value);
            return Ok();
        }

        [HttpPut("UpdateTask{taskId:guid}")]
        public async Task<ActionResult> UpdateTask(Guid taskId, [FromBody] InterviewCreateRequest newTask)
        {
            Console.WriteLine("Изменяю");
            await _tasksService.Update(taskId, newTask);
            return Ok();
        }

        [HttpDelete("DeleteTask{taskId:guid}")]
        public async Task<ActionResult> DeleteTask(Guid taskId)
        {
            Console.WriteLine("Удаляю");
            await _tasksService.Delete(taskId);
            return Ok();
        }*/

    }
}
