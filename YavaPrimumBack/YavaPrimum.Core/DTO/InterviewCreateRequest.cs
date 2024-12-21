namespace YavaPrimum.Core.DTO
{
    public record InterviewCreateRequest(
        CandidateRequestResponse Candidate,
        DateTime InterviewDate
    );
}
