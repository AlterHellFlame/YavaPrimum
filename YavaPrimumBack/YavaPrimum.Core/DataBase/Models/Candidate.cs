namespace YavaPrimum.Core.DataBase.Models
{
    public class Candidate
    {
        public Guid CandidateId { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SurName { get; set; } //Отчество
        public string Post { get; set; }
        public string Country { get; set; }
    }
}
