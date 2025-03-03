namespace YavaPrimum.Core.DataBase.Models
{
    public class ArchiveTasks
    {
        public Guid ArchiveTasksId { get; set; }
        public Tasks Task { get; set; }
        public TasksStatus Status { get; set; }
        public DateTime DateTimeOfCreated { get; set; }
    }
}
