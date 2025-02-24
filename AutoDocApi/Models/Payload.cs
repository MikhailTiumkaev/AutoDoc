namespace  AutoDocApi.Models
{
    public class Payload
    {
        public int Id { get; set; }
        public int TodoTaskId { get; set; }    
        public required string PayloadLocation { get; set; }
            
    }
}