using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface IConverterService
    {
        Task<List<TasksResponse>> ConvertToFront(List<Tasks> tasks);
        Task<TasksResponse> ConvertToFront(Tasks task);
        Task<UserRequestResponse> ConvertToFront(User user);
        Task<List<UserRequestResponse>> ConvertToFront(List<User> users);
    }
}
