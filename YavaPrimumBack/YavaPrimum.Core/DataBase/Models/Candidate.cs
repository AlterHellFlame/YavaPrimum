namespace YavaPrimum.Core.DataBase.Models
{
    public class Candidate
    {
        public Guid CandidateId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SurName { get; set; } //Отчество
        public string Telephone { get; set; }
        public string Email { get; set; }

        public Post Post { get; set; }
        public Country Country { get; set; }
        public User HR { get; set; }
        public User? OP { get; set; }
    }
}
