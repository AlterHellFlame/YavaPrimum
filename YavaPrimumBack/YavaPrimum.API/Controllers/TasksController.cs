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

        public TasksController(ITasksService tasksService,
            YavaPrimumDBContext dBContext,
            ICandidateService candidateService)
        {
            _tasksService = tasksService;
            _dBContext = dBContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tasks>>> GetTasks()
        {
            return Ok(await _tasksService.GetAll());
        }


        [HttpPost("Interview/{taskId:guid}")]
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
        }

    }
}
