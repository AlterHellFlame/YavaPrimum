using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class CandidateService : ICandidateService
    {
        private YavaPrimumDBContext _dBContext;
        private ICompanyService _companyService;
        private ICountryService _countryService;
        private IPostService _postService;
        private readonly IConverterService _converterService;

        public CandidateService(YavaPrimumDBContext dBContext, 
            ICompanyService companyService, 
            ICountryService countryService, 
            IPostService postService, 
            IConverterService converterService)
        {
            _dBContext = dBContext;
            _companyService = companyService;
            _countryService = countryService;
            _postService = postService;
            _converterService = converterService;
        }



        /*public async Task<List<CandidateRequestResponse>> GetAll()
        {
            return await _dBContext.Candidate
                .Select(c => new CandidateRequestResponse(
                    c.CandidateId,
                    c.FirstName,
                    c.SecondName,
                    c.SurName,
                    c.Email,
                    c.Telephone,
                    c.Post.Name,
                    c.Country.Name,
                    c.InterviewStatus
                ))
                .ToListAsync();
        }*/


        public async Task<Candidate> GetById(Guid id)
        {
            return new Candidate(); /*await _dBContext.Candidate
                .Include(c => c.Post)
                .Include(c => c.Country)
                .Include(c => c.HR)
                .Include(c => c.OP)
                .FirstOrDefaultAsync(c => c.CandidateId == id);*/
        }

        public async Task<Guid> Create(Candidate candidate)
        {
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();

            return candidate.CandidateId;
        }

        public async Task<Guid> Create(CandidateRequestResponse candidateRequest)
        {
            /*Candidate candidate = new Candidate()
            {
                CandidateId = Guid.NewGuid(),
                FirstName = candidateRequest.FirstName,
                SecondName = candidateRequest.SecondName,
                SurName = candidateRequest.SurName,
                Email = candidateRequest.Email,
                Telephone = candidateRequest.Telephone,
                Country = await _countryService.GetByName(candidateRequest.Country),
                Post = await _postService.GetByName(candidateRequest.Post),
                InterviewStatus = candidateRequest.InterviewStatus
            };

            Console.WriteLine("Попытка добавить кандидата " + candidate.SecondName);
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();*/

            return new Guid();//candidate.CandidateId;
        }

        public async Task<Guid> Update(Candidate candidate, CandidateRequestResponse candidateRequest)
        {
            /*candidate.SecondName = candidateRequest.SecondName;
            candidate.FirstName = candidateRequest.FirstName;
            candidate.SurName = candidateRequest.SurName;
            candidate.Email = candidateRequest.Email;
            candidate.Telephone = candidateRequest.Telephone;
            candidate.Country = await _countryService.GetByName(candidateRequest.Country);
            candidate.Post = await _postService.GetByName(candidateRequest.Post);
            candidate.InterviewStatus = candidateRequest.InterviewStatus;

            await _dBContext.SaveChangesAsync();*/

            return new Guid(); //candidate.CandidateId;
        }

        public async Task<List<CandidateRequestResponse>> GetAll()
        {
            // Получаем список кандидатов из базы данных
            List<Candidate> candidates = await _dBContext.Candidate
                .Include(c => c.Country)
                .ToListAsync();

            // Преобразуем список кандидатов в список CandidateRequestResponse
            List<CandidateRequestResponse> candidatesList = candidates.Select(c => new CandidateRequestResponse
            {
                Surname = c.Surname,
                FirstName = c.FirstName,
                Patronymic = c.Patronymic,
                Email = c.Email,
                Phone = c.Phone,
                Country = c.Country.Name // Преобразуем объект Country в строку (если Country не null)
            }).ToList();

            return candidatesList;
        }

        public async Task<List<CandidateFullDataResponse>> GetAllCandidatesFullData(Guid userId)
        {
            // 1. Получаем всех кандидатов, связанных с пользователем через задачи
            var userCandidateIds = await _dBContext.Tasks
                .Where(t => t.User.UserId == userId)
                .Select(t => t.Candidate.CandidateId)
                .Distinct()
                .ToListAsync();

            if (userCandidateIds == null || !userCandidateIds.Any())
            {
                throw new InvalidOperationException("У данного пользователя нет связанных кандидатов.");
            }

            // 2. Получаем полные данные этих кандидатов
            var candidates = await _dBContext.Candidate
                .Where(c => userCandidateIds.Contains(c.CandidateId))
                .Include(c => c.Post)
                .Include(c => c.Country)
                .ToListAsync();

            // 3. Получаем ВСЕ задачи этих кандидатов (не только текущего пользователя)
            var allTasks = await _dBContext.Tasks
                .Where(t => userCandidateIds.Contains(t.Candidate.CandidateId))
                .Include(t => t.Status)
                .Include(t => t.User)
                .Include(t => t.User.Post)
                .Include(t => t.User.Company)
                .Include(t => t.User.Company.Country)
                .Include(t => t.Candidate)
                .OrderBy(t => t.DateTime)
                .ToListAsync();

            // 4. Формируем ответ
            var responseList = new List<CandidateFullDataResponse>();

            foreach (var candidate in candidates)
            {
                var candidateTasks = allTasks
                    .Where(t => t.Candidate.CandidateId == candidate.CandidateId)
                    .OrderBy(t => t.DateTime)
                    .ToList();

                var response = new CandidateFullDataResponse
                {
                    Candidate = new CandidateRequestResponse
                    {
                        Surname = candidate.Surname,
                        FirstName = candidate.FirstName,
                        Patronymic = candidate.Patronymic,
                        Email = candidate.Email,
                        Phone = candidate.Phone,
                        Post = candidate.Post?.Name ?? "Должность не указана",
                        Country = candidate.Country?.Name ?? "Страна не указана"
                    },
                    Tasks = await _converterService.ConvertToFront(candidateTasks),
                };

                responseList.Add(response);
            }

            return responseList;
        }



    }
}
