namespace TailorrNow.Models
{
    public class Commission
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateRecorded { get; set; }
    }
}
