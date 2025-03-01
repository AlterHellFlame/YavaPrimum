using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Metrics;
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

        public HRController(IHRService hRService,
            ICandidateService candidateService,
            ICountryService countryService,
            IPostService postService,
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService)
        {
            _HRService = hRService;
            _candidateService = candidateService;
            _countryService = countryService;
            _postService = postService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
        }

        /*[HttpPost("/create-task")]
        public async Task<ActionResult<Guid>> CreateNewIntreview([FromBody] InterviewCreateRequest request)
        {
            Console.WriteLine(request.InterviewDate.ToString() + " " + request.Candidate.Surname);
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
                DateTime = DateTime.Parse(request.InterviewDate),,
                User = candidate.HR
            };

            await _candidateService.Create(candidate);
            await _tasksService.Create(task);

            return Ok(new Guid());//Ok(task.TasksId);
        }*/

        [HttpPost("/change-task")]
        public async Task<ActionResult<Guid>> ChangeNewIntreview([FromBody] InterviewCreateRequest request)
        {
            Console.WriteLine(request.InterviewDate.ToString() + " " + request.Candidate.Surname);
            
            return Ok(new Guid());
        }


    }
}
