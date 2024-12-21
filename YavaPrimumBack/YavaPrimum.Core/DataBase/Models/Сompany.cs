namespace YavaPrimum.Core.DataBase.Models
{
    public class Company
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}
