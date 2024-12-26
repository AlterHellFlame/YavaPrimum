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
        private ICompanyService _companyService;
        private ICountryService _countryService;
        private IPostService _postService;
        private ITaskTypeService _taskTypeService;

        public CandidateService(YavaPrimumDBContext dBContext, 
            ICompanyService companyService, 
            ICountryService countryService, 
            IPostService postService, 
            ITaskTypeService taskTypeService)
        {
            _dBContext = dBContext;
            _companyService = companyService;
            _countryService = countryService;
            _postService = postService;
            _taskTypeService = taskTypeService;
        }

        public async Task<List<CandidateRequestResponse>> GetAll()
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
        }

        public async Task<Candidate> GetById(Guid id)
        {
            return await _dBContext.Candidate
                .Include(c => c.Post)
                .Include(c => c.Country)
                .Include(c => c.HR)
                .Include(c => c.OP)
                .FirstOrDefaultAsync(c => c.CandidateId == id);
        }

        public async Task<Guid> Create(Candidate candidate)
        {
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();

            return candidate.CandidateId;
        }

        public async Task<Guid> Create(CandidateRequestResponse candidateRequest)
        {
            Candidate candidate = new Candidate()
            {
                CandidateId = Guid.NewGuid(),
                FirstName = candidateRequest.FirstName,
                SecondName = candidateRequest.SecondName,
                SurName = candidateRequest.SurName,
                Email = candidateRequest.Email,
                Telephone = candidateRequest.Telephone,
                Country = await _countryService.GetByName(candidateRequest.Country),
                Post = await _postService.GetByName(candidateRequest.Post),
            };

            Console.WriteLine("Попытка добавить кандидата " + candidate.SecondName);
            await _dBContext.Candidate.AddAsync(candidate);
            await _dBContext.SaveChangesAsync();

            return candidate.CandidateId;
        }
    }
}
