using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace YavaPrimum.Core.Services
{
    public class UserService : IUserService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly ITasksService _tasksService;

        public UserService(YavaPrimumDBContext dbContext, ITasksService tasksService)
        {
            _dbContext = dbContext;
            _tasksService = tasksService;
        }

        public async Task<User> GetByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(p => p.Post)
                .FirstOrDefaultAsync(
                u => u.Email == email);

            if (user == null)
                throw new ArgumentNullException("Пользователя с такой почтой не существует");

            return user;
        }

        public async Task<bool> IsUserExistByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(p => p.Post)
                .FirstOrDefaultAsync(
                u => u.Email == email);

            if (user == null)
                return false;

            return true;
        }

        public async Task<User> GetById(Guid id)
        {
            return await _dbContext.User
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _dbContext.User
                .Include(p => p.Post)
                .Include(c => c.Company)
                .Include(co => co.Company.Country)
                .ToListAsync();
        }

        public async Task UpdateUser(User user)
        {
            _dbContext.User.Update(user);

            _dbContext.SaveChanges();

        }



        public async Task DeleteUserById(Guid id)
        {
            var user = await _dbContext.User.FindAsync(id); // Получение пользователя по идентификатору

            if (user != null)
            {
                _dbContext.User.Remove(user); // Удаление пользователя из контекста
                await _dbContext.SaveChangesAsync(); // Сохранение изменений
            }
        }



        public async Task<UserRequestResponse> GetByIdToFront(Guid id)
        {
            User user = await _dbContext.User
                .Include(p => p.Post)
                .Include(c => c.Company.Country)
                .Where(u => u.UserId == id)
                .FirstAsync();

            if (user == null)
                throw new ArgumentNullException("Пользователя с таким ID не существует");

            UserRequestResponse userResponse = await ConvertToFront(user);

            return userResponse;
        }

        public async Task<UserRequestResponse> ConvertToFront(User user)
        {
            UserRequestResponse userResponse =  new UserRequestResponse()
            {
                UserId = user.UserId,
                Surname = user.Surname,
                FirstName = user.FirstName,
                Patronymic = user.Patronymic,
                Post = user.Post.Name,
                Company = user.Company.Name,
                Email = user.Email,
                Phone = user.Phone,
                ImgUrl = user.ImgUrl
            };

            return userResponse;
        }

        public async Task<List<UserRequestResponse>> ConvertToFront(List<User> users)
        {
            List<Task<UserRequestResponse>> usersRequestResponse = users.Select(user => ConvertToFront(user)).ToList();
            return (await Task.WhenAll(usersRequestResponse)).ToList();
        }
    }
}
