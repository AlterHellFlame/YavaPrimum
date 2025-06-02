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
    public class NotificationController : ControllerBase
    {
        private IUserService _userService;
        private IJwtProvider _jwtProvider;
        private ITasksService _tasksService;
        private INotificationsService _notificationsService;
        private IConverterService _converterService;

        public NotificationController(
            IUserService userService,
            IJwtProvider jwtProvider,
            ITasksService tasksService,
            INotificationsService notificationsService,
            IConverterService converterService)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
            _tasksService = tasksService;
            _notificationsService = notificationsService;
            _converterService = converterService;
        }

        [HttpGet("/get-notifications")]
        public async Task<ActionResult<List<NotificationsResponse>>> GetNotifications()
        {
            if (HttpContext.Request.Cookies.Count == 0)
            {
                Console.WriteLine("Куков нет");
                return Ok(new List<TasksResponse>());
            }
            string token = HttpContext.Request.Cookies[JwtProvider.CookiesName];

            return await _notificationsService.GetAllByUserId(await _jwtProvider.GetUserIdFromToken(token));
        }

        [HttpPut("/read-notification/{notificationId:guid}")]
        public async Task<ActionResult> ReadNotification(Guid notificationId)
        {

            //Console.WriteLine("Сообщение прочитано");

            Notifications notification = await _notificationsService.GetById(notificationId);
            await _notificationsService.ReadNotification(notificationId);
            return Ok();
        }

    }
}
