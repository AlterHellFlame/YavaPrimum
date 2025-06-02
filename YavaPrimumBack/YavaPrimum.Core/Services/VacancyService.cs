using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly YavaPrimumDBContext _dbContext;
        private readonly IConverterService _converterService;
        private readonly IPostService _postService;

        public VacancyService(YavaPrimumDBContext dbContext, IConverterService converterService, IPostService postService)
        {
            _dbContext = dbContext;
            _converterService = converterService;
            _postService = postService;
        }

        public async Task<List<Vacancy>> GetAll()
        {
            List<Vacancy>? vacancies = await _dbContext.Vacancy
                .Include(p => p.Post)
                .Include(p => p.User)
                .Include(p => p.User.Company)
                .Include(p => p.User.Company.Country)
                .ToListAsync();

            return vacancies;
        }

        public async Task Close(Guid id)
        {
            Vacancy? vacancy = await _dbContext.Vacancy
                       .FirstOrDefaultAsync(v => v.VacancyId == id);


            vacancy.isClose = true;
            await _dbContext.SaveChangesAsync();

            return;
        }

        public async Task Update(Guid id, VacancyRequest request)
        {
            Vacancy? vacancy = await _dbContext.Vacancy
          .FirstOrDefaultAsync(v => v.VacancyId == id);

            vacancy.Post = await _postService.GetByName(request.Post);
            vacancy.Count = request.Count;
            vacancy.AdditionalData = request.AdditionalData;

            await _dbContext.SaveChangesAsync();

            return;

        }
        public async Task Create(VacancyRequest vacancy, User user)
        {

            // Проверяем, существует ли такая должность (Post) в базе
            var post = await _dbContext.Post
                .FirstOrDefaultAsync(p => p.Name == vacancy.Post);

            // Если должности нет, создаём новую
            if (post == null)
            {
                post = new Post
                {
                    PostId = Guid.NewGuid(),
                    Name = vacancy.Post
                };
                await _dbContext.Post.AddAsync(post);
            }

            // Создаём новую вакансию
            var newVacancy = new Vacancy
            {
                VacancyId = Guid.NewGuid(),
                Post = post,
                Count = vacancy.Count,
                isClose = false,
                AdditionalData = vacancy.AdditionalData,
                // User можно установить позже или оставить null, если вакансия не привязана к пользователю
                User = user
            };

            // Добавляем в контекст и сохраняем
            await _dbContext.Vacancy.AddAsync(newVacancy);
            await _dbContext.SaveChangesAsync();

            return;
        }
    }
}
