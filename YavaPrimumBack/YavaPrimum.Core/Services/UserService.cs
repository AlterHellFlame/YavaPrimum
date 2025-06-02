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
        private readonly IConverterService _converterService;

        public UserService(YavaPrimumDBContext dbContext, IConverterService converterService)
        {
            _dbContext = dbContext;
            _converterService = converterService;
        }

        public async Task<User> GetByEMail(string email)
        {
            User? user = await _dbContext.User
                .Include(p => p.Post)
                .FirstOrDefaultAsync(
                u => u.Email == email);

            if (user == null)
            {
                throw new ArgumentNullException("Пользователя с такой почтой не существует");
            }

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
                .Include(p => p.Post)
                .Include(c => c.Company)
                .Include(co => co.Company.Country)
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAll()
        {
            Console.WriteLine("GetAll");
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
            Console.WriteLine("GetByIdToFront");
            User user = await _dbContext.User
                .Include(p => p.Post)
                .Include(c => c.Company)
                .Include(c => c.Company.Country)
                .Where(u => u.UserId == id)
                .FirstOrDefaultAsync();

            if (user == null)
                throw new ArgumentNullException("Пользователя с таким ID не существует");

            UserRequestResponse userResponse = await _converterService.ConvertToFront(user);

            return userResponse;
        }

        public async Task<User> GetAnotherUserOfCandidate(Tasks task)
        {
            User? user = _dbContext.Tasks
             .Include(u => u.User)
             .Include(u => u.User.Post)
             .Where(u => u.Candidate == task.Candidate && u.User.Post != task.User.Post)
             .FirstOrDefault().User;
            

            if (user == null)
                throw new ArgumentNullException("Пользователя с таким ID не существует");

            return user;
        }

    }
}
