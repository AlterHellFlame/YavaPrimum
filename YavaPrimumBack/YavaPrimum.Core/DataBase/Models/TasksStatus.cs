namespace YavaPrimum.Core.DataBase.Models
{
    public class TasksStatus
    {
        public Guid TasksStatusId { get; set; }
        public string Name { get; set; }
        public int TypeStatus { get; set; }
        public string? MessageTemplate { get; set; }

    }
}
