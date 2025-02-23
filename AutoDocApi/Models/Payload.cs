namespace  AutoDocApi.Models
{
    public class Payload
    {
        public int Id { get; set; }
        public byte[] Content { get; set; } = default!;
    }
}