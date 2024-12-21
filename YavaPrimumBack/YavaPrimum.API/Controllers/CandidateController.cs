using Microsoft.AspNetCore.Mvc;
using YavaPrimum.Core.DTO;
using YavaPrimum.Core.Interfaces;
using YavaPrimum.Core.Services;

[ApiController]
[Route("api/[controller]")]
public class CandidateController : ControllerBase
{
    private readonly ICandidateService _candidateService;

    public CandidateController(ICandidateService candidateService)
    {
        _candidateService = candidateService;
    }

    [HttpPost]
    public async Task<Guid> CreateCandidat([FromBody] CandidateRequestResponse candidateRequest)
    {
        return await _candidateService.Create(candidateRequest);
    }

    [HttpGet]
    public async Task<IActionResult> GetCandidates()
    {
        return Ok(await _candidateService.GetAll());
    }

}
