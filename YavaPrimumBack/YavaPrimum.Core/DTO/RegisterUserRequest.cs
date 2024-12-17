namespace YavaPrimum.Core.DTO
{
    public record RegisterUserRequest(
        string FirstName,
        string SecondName,
        string SurName,
        string Email,

        Guid PostId
    );
}
