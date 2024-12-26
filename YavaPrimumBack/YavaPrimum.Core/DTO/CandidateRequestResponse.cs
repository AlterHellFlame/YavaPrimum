namespace YavaPrimum.Core.DTO
{
    public record CandidateRequestResponse(
        Guid CandidateId,
        string FirstName,
        string SecondName,
        string SurName,
        string Email,
        string Telephone,
        string Post,
        string Country,
        int InterviewStatus
    );
}

