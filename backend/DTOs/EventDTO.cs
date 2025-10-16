namespace backend.DTOs
{
    public class EventDTO
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public double Price { get; set; }
        public int AvailableSeats { get; set; }
    }
}
