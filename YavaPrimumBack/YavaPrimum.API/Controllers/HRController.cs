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

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateNewIntreview([FromBody] InterviewCreateRequest request)
        {
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
                DateTime = request.InterviewDate,
                TaskType = await _taskTypeService.GetByName("Interview"),
                User = candidate.HR
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);

            return Ok(task.TasksId);
        }

        [HttpGet]
        public async Task<ActionResult<Tasks>> GetHRTasks()
        {
            return Ok(await _tasksService.GetAllByUserId(
                await _jwtProvider.GetUserIdFromToken(
                    HttpContext.Request.Cookies[JwtProvider.CookiesName])));
        }
    }
}
