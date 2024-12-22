namespace YavaPrimum.Core.DTO
{
    public record CandidateRequestResponse(
        string FirstName,
        string SecondName,
        string SurName,
        string Email,
        string Telephone,
        string Post,
        string Country
    );
}

