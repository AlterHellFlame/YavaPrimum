using YavaPrimum.Core.DataBase.Models;
using YavaPrimum.Core.DTO;

namespace YavaPrimum.Core.Interfaces
{
    public interface ICandidateService
    {
        Task<Guid> Create(Candidate candidate);
        Task<Guid> Create(CandidateRequestResponse candidateRequest);
        Task<List<CandidateRequestResponse>> GetAll();
        Task<Candidate> GetById(Guid id);
        Task<Guid> Update(Candidate candidate, CandidateRequestResponse candidateRequest);
        Task<List<CandidateFullDataResponse>> GetAllCandidatesFullData(Guid userId);

    }
}