using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase;
using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;

namespace YavaPrimum.Core.Services
{
    public class CandidateService : ICandidateService
    {
        private YavaPrimumDBContext _dBContext;

        public CandidateService(YavaPrimumDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<List<CandidateRequest>> GetAll()
        {
            return await _dBContext.Candidate
                .Select(c => new CandidateRequest
                {
                    FirstName = c.FirstName,
                    SecondName = c.SecondName,
                    SurName = c.SurName,
                    Post = c.Post,
                    Country = c.Country
                })
                .ToListAsync();
        }

        public async Task<Candidate> GetById(Guid id)
        {
            return await _dBContext.Candidate
                //.Include(c=>c.SecondName)
                .FirstOrDefaultAsync(c => c.CandidateId == id);
        }

        public async Task<Guid> Create(Candidate candidate)
        {
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();

            return candidate.CandidateId;
        }

        public async Task<Guid> Create(CandidateRequest candidateRequest)
        {
            Candidate candidate = new Candidate()
            {
                CandidateId = Guid.NewGuid(),
                Country = candidateRequest.Country,
                Post = candidateRequest.Post,
                FirstName = candidateRequest.FirstName,
                SecondName = candidateRequest.SecondName,
                SurName = candidateRequest.SurName,
            };
            Console.WriteLine("Попытка добавить кандидата " + candidate.SecondName);
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();
            return candidate.CandidateId;
        }
    }
}
