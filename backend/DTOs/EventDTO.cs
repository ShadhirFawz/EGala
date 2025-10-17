namespace backend.DTOs
{
    public class TicketPackageDTO
    {
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public int Capacity { get; set; }
        public List<string> Materials { get; set; } = new();
    }

    public class SeatDTO
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsBooked { get; set; } = false;
    }

    public class EventDTO
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        // Coordinates for map
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; } = null!;
        public DateTime Date { get; set; }
        public List<TicketPackageDTO> TicketPackages { get; set; } = new();
        public List<SeatDTO>? SeatLayout { get; set; }
        public List<string>? Images { get; set; }
        public bool IsPublished { get; set; } = false;
    }
}
