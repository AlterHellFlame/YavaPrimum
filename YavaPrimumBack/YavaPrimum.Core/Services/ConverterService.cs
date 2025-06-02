using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class ConverterService : IConverterService
    {

        public async Task<List<TasksResponse>> ConvertToFront(List<Tasks> tasks)
        {
            List<Task<TasksResponse>> taskResponseTasks = tasks.Select(task => ConvertToFront(task)).ToList();
            TasksResponse[] taskResponses = await Task.WhenAll(taskResponseTasks);
            return taskResponses.ToList();
        }

        public async Task<TasksResponse> ConvertToFront(Tasks task)
        {
            TasksResponse tasksResponse = new TasksResponse
            {
                TaskId = task.TasksId,
                Status = task.Status.Name,
                TypeStatus = task.Status.TypeStatus,
                DateTime = task.DateTime,
                User = new UserRequestResponse()
                {
                    UserId = task.User.UserId,
                    Surname = task.User.Surname,
                    FirstName = task.User.FirstName,
                    Patronymic = task.User.Patronymic,
                    Post = task.User.Post.Name,
                    Company = task.User.Company.Name,
                    Email = task.User.Email,
                    Phone = task.User.Phone,
                    ImgUrl = task.User.ImgUrl
                },
                Candidate = new CandidateRequestResponse
                {
                    Surname = task.Candidate.Surname,
                    FirstName = task.Candidate.FirstName,
                    Patronymic = task.Candidate.Patronymic,
                    Email = task.Candidate.Email,
                    Phone = task.Candidate.Phone,
                    Post = task.Candidate.Post.Name,
                    Country = task.Candidate.Country.Name
                },
                AdditionalData = task.AdditionalData,
                IsArchive = task.IsArchive              
            };

            return tasksResponse;
        }

        public async Task<UserRequestResponse> ConvertToFront(User user)
        {
            UserRequestResponse userResponse = new UserRequestResponse()
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

        public async Task<VacancyResponce> ConvertToFront(Vacancy vacancy)
        {
            if (vacancy == null)
                return null;

            // Преобразование User в UserRequestResponse (если User не null)
            var userResponse = vacancy.User != null
                ? new UserRequestResponse
                {
                    UserId = vacancy.User.UserId,
                    Surname = vacancy.User.Surname,
                    FirstName = vacancy.User.FirstName,
                    Patronymic = vacancy.User.Patronymic,
                    ImgUrl = vacancy.User.ImgUrl,
                    Phone = vacancy.User.Phone,
                    Email = vacancy.User.Email,
                    Company = vacancy.User.Company?.Name, // Предполагается, что Company имеет свойство Name
                    Post = vacancy.User.Post?.Name // Предполагается, что Post имеет свойство Name
                }
                : null;

            return new VacancyResponce
            {
                VacancyId = vacancy.VacancyId,
                User = userResponse,
                Post = vacancy.Post?.Name, // Если Post не null, берём Name
                Count = vacancy.Count,
                isClose = vacancy.isClose,
                AdditionalData = vacancy.AdditionalData
            };
        }

        public async Task<List<VacancyResponce>> ConvertToFront(List<Vacancy> vacancies)
        {
            if (vacancies == null)
                return new List<VacancyResponce>();

            var tasks = vacancies.Select(v => ConvertToFront(v)).ToList();
            return (await Task.WhenAll(tasks)).ToList();
        }


    }
}
