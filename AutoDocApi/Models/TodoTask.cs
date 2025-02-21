namespace AutoDocApi.Models
{
    public class TodoTask
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Status { get; set; }
        public DateTime DueDate { get; set; }
    }
}