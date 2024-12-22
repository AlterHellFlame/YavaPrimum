using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRController : ControllerBase
    {
        private IHRService _HRService;
        private ICandidateService _candidateService;
        private ICountryService _countryService;
        private IPostService _postService;
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private ITaskTypeService _taskTypeService;

        public HRController(IHRService hRService,
            ICandidateService candidateService,
            ICountryService countryService,
            IPostService postService,
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService,
            ITaskTypeService taskTypeService)
        {
            _HRService = hRService;
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _taskTypeService = taskTypeService;
        }

        [HttpPost("/create-task")]
        public async Task<ActionResult<Guid>> CreateNewIntreview([FromBody] InterviewCreateRequest request)
        {
            Console.WriteLine(request.InterviewDate.ToString() + " " + request.Candidate.SurName);
            Candidate candidate = new Candidate()
            {
                CandidateId = Guid.NewGuid(),
                FirstName = request.Candidate.FirstName,
                SecondName = request.Candidate.SecondName,
                SurName = request.Candidate.SurName,
                Telephone = request.Candidate.Telephone,
                Email = request.Candidate.Email,

                Country = await _countryService.GetByName(request.Candidate.Country),
                Post = await _postService.GetByName(request.Candidate.Post),
                HR = await _userService.GetById(await _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName]))
            };

            Tasks task = new Tasks()
            {
                TasksId = Guid.NewGuid(),
                Candidate = candidate,
                DateTime = DateTime.Parse(request.InterviewDate),
                TaskType = await _taskTypeService.GetByName("Интервью"),
                User = candidate.HR
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);

            return Ok(task.TasksId);
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskResponse>>> GetHRTasks()
        {

            if(HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TaskResponse>());
            }
            string tokenus = HttpContext.Request.Cookies[JwtProvider.CookiesName];
            List<Tasks> tasks = await _tasksService.GetAllByUserId(
                await _jwtProvider.GetUserIdFromToken(tokenus));

            List<TaskResponse> taskResponses = new List<TaskResponse>();

            foreach (var task in tasks)
            {
                taskResponses.Add(new TaskResponse(
                    task.Status,
                    task.DateTime,
                    new CandidateRequestResponse(
                        task.Candidate.FirstName,
                        task.Candidate.SecondName,
                        task.Candidate.SurName,
                        task.Candidate.Email,
                        task.Candidate.Telephone,
                        task.Candidate.Post.Name,
                        task.Candidate.Country.Name
                    ),
                    task.TaskType.Name
                ));
            }

            return Ok(taskResponses);
        }
    }
}
