namespace YavaPrimum.Core.DTO
{
    public record TaskResponse(
        Guid TaskResponseId,
        
        bool Status,
        DateTime DateTime,
        string TaskType,

        CandidateRequestResponse Candidate

    );
}
