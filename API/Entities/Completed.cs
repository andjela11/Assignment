namespace API.Entities
{
    public class Completed 
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DateCompleted { get; set; }
        public Employee? Assignee { get; set; }

    }
}