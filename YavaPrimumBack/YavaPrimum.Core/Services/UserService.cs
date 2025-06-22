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
            User? user = await _dbContext.User.FindAsync(id);
            if (user == null)
                return;

            user.Email = "-";

            var excludedStatuses = new[] { "Пришел", "Не пришел", "Собеседование не пройдено", "Не выполнено тестовое задание" };

            var candidatesToDelete = await _dbContext.Candidate
                .Where(candidate => !_dbContext.Tasks
                    .Where(task => task.User == user && task.CandidateId == candidate.CandidateId)
                    .Any(task => excludedStatuses.Contains(task.Status.Name))) // Если есть хотя бы одна, кандидат исключается
                .ToListAsync();

            _dbContext.Candidate.RemoveRange(candidatesToDelete);

            var notificationsToDelete = await _dbContext.Notifications
                .Where(n => candidatesToDelete.Contains(n.Task.Candidate))
                .ToListAsync();
            _dbContext.Notifications.RemoveRange(notificationsToDelete);

            var tasksToDelete = await _dbContext.Tasks
                .Where(n => candidatesToDelete.Contains(n.Candidate))
                .ToListAsync();
            _dbContext.Tasks.RemoveRange(tasksToDelete);

            await _dbContext.SaveChangesAsync();
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
