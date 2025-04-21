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
                }
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

    }
}
