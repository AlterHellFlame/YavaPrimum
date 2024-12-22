namespace YavaPrimum.Core.DTO
{
    public record TaskResponse(
        bool Status,
        DateTime DateTime,
        CandidateRequestResponse Candidate,
        string TaskType
    );
}
