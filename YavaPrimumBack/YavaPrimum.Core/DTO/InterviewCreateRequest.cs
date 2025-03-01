namespace YavaPrimum.Core.DTO
{
    public record InterviewCreateRequest(
        CandidateRequestResponse Candidate,
        string InterviewDate,
        string Post
    );
}
