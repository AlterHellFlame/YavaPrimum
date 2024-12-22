using Microsoft.AspNetCore.SignalR;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.API.Notify
{
    public class NotifyHub : Hub
    {
        private readonly IUserService _usersService;

        public NotifyHub(IUserService usersService)
        {
            _usersService = usersService;
        }

        public async Task Send(string message)
        {
            Console.WriteLine("Сообщение для кадровика: " + message);
            var users = await _usersService.GetAll();

            var filteredUsers = users.Where(user => user.Company.Country.Name == "Беларусь" && user.Post.Name != "HR");

            // Отправка сообщения фильтрованным пользователям
            foreach (var user in filteredUsers)
            {
                await this.Clients.User(user.UserId.ToString()).SendAsync("Receive", message);
            }
        }

        /*


        public async Task Send(string message)
        {
            Console.WriteLine("Сообщение для кадровика: " + message);
            await this.Clients.All.SendAsync("Receive", message);
            
            var users = await _usersService.GetAll();

            var filteredUsers = users.Where(user => user.Company.Country.Name == "Беларусь" && user.Post.Name != "HR");

            // Отправка сообщения фильтрованным пользователям
            foreach (var user in filteredUsers)
            {
                await this.Clients.All.SendAsync("Receive", message);
            }
        }*/
    }
}
