namespace YavaPrimum.Core.DTO
{
    public record TasksRequest(

        CandidateRequestResponse Candidate,
        string Status,
        string Post,
        DateTime? DateTime,
        string? AdditionalData
    );

}
