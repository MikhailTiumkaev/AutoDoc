namespace  AutoDocApi.Models
{
    public class Payload
    {
        public Guid Id { get; set; }
        public byte[] Content { get; set; } = default!;
    }
}