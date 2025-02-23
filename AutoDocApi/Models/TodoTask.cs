namespace AutoDocApi.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Status { get; set; }
        public DateTime DueDate { get; set; }
        public List<Payload> Payloads { get; set; } = default!;
    }
}