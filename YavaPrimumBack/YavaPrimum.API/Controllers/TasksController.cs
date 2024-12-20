using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

namespace YavaPrimum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        //private readonly YavaPrimumDBContext _dBContext;

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            return Ok(await _tasksService.GetAll());
        }
    }
}
