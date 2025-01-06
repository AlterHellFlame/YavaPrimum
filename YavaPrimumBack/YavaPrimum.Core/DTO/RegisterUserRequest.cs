namespace YavaPrimum.Core.DTO
{
    public record RegisterUserRequest(
        string FirstName,
        string SecondName,
        string SurName,
        string ImgUrl,
        string Phone,
        string Company,
        string Email,
        string Post
    );
}
