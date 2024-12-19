using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.Services
{
    internal class TaskService
    {
        private readonly YavaPrimumDBContext _dbContext;

        //public async Task<List<Models.Task>> GetAll()
        //{
        //    return await _dbContext.Task.ToListAsync();
        //}
    }
}
