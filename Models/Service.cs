namespace TailorrNow.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int TailorId { get; set; } 
        public string ServiceName { get; set; } = string.Empty;
        public decimal Price { get; set; } 
    }
}
