using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly YavaPrimumDBContext _dBContext;
        private readonly ITasksService _tasksService;
        private readonly ICandidateService _candidateService;

        public TasksController(ITasksService tasksService,
            YavaPrimumDBContext dBContext,
            ICandidateService candidateService)
        {
            _tasksService = tasksService;
            _dBContext = dBContext;
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tasks>>> GetTasks()
        {
            return Ok(await _tasksService.GetAll());
        }


        [HttpPost("PassedInterview{taskId:guid}")]
        public async Task<ActionResult> CommitTask(Guid taskId)
        {
            await _tasksService.PassedInterview(taskId);
            return Ok();
        }

        [HttpPut("RepeatInterview{taskId:guid}")]
        public async Task<ActionResult> RepeatInterview(Guid taskId, string dateTime)
        {
            await _tasksService.RepeatInterview(taskId, dateTime);

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
        }


        [HttpGet("FillTables")]//Заполняет таблички, где только название и id
        public async Task<ActionResult> FillTables()
        {
            Country country = new Country()
            {
                CountryId = Guid.NewGuid(),
                Name = "Россия"
            };

            Post post = new Post()
            {
                PostId = Guid.NewGuid(),
                Name = "Кадровик"
            };

            TaskType taskType = new TaskType()
            {
                TaskTypeId = Guid.NewGuid(),
                Name = "Звонок"
            };

            Company company = new Company()
            {
                CompanyId = Guid.NewGuid(),
                Name = "Первый Элемент",
                Country = country
            };

            _dBContext.Post.Add(post);
            _dBContext.TaskType.Add(taskType);
            _dBContext.Country.Add(country);
            _dBContext.Company.Add(company);
            _dBContext.SaveChanges();
            return Ok();
        }
    }
}
