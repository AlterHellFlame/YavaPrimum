using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.API.Notify
{
    public class NotifyHub : Hub
    {
        private readonly IUserService _usersService;
        private readonly IJwtProvider _jwtProvider;

        public NotifyHub(IUserService usersService, IJwtProvider jwtProvider)
        {
            _usersService = usersService;
            _jwtProvider = jwtProvider;
        }

        public async Task SendToUser(string message)
        {
            Console.WriteLine("Сообщение для кадровика: " + message);
            await this.Clients.User("2C0BA6CA-F14C-4733-B567-A1D851314259".ToLower()).SendAsync("Receive", message);
        }

        public async Task SendToCountry(string message)
        {
            Console.WriteLine("Сообщение для кадровика: " + message);
            await this.Clients.User("2C0BA6CA-F14C-4733-B567-A1D851314259".ToLower()).SendAsync("Receive", message);
        }

        public async Task Grouping(string message)
        {
            await Groups.AddToGroupAsync("2C0BA6CA-F14C-4733-B567-A1D851314259".ToLower(), "Страна");
        }

    }

    public class UserIdProvider : IUserIdProvider
    {
        private readonly IJwtProvider _jwtProvider;

        public UserIdProvider(IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var cookies = connection.GetHttpContext().Request.Cookies;
            if (cookies.TryGetValue("token-cookies", out var token))
            {
                var userId = _jwtProvider.GetUserIdFromToken(token).Result.ToString();
                return userId;
            }
            return null;
        }
    }
}
