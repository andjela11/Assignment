namespace API.Entities
{
    public class WorkTask
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Employee? Assignee { get; set; }
    }
}