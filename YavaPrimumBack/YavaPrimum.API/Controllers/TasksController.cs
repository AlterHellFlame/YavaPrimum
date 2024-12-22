using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly YavaPrimumDBContext _dBContext;

        public TasksController(ITasksService tasksService,
            YavaPrimumDBContext dBContext)
        {
            _tasksService = tasksService;
            _dBContext = dBContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tasks>>> GetTasks()
        {
            return Ok(await _tasksService.GetAll());
        }

        [HttpGet("/FillTables")]//Заполняет таблички, где только название и id
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
