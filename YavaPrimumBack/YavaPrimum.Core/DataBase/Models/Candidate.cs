namespace YavaPrimum.Core.DataBase.Models
{
    public class Candidate
    {
        public Guid CandidateId { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; } //Отчество

        public string Phone { get; set; }
        public string Email { get; set; }

        public Country Country { get; set; }
    }
}
