namespace frontend.Models
{
    public class EventLocationDTO
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class SeatMatrixDTO
    {
        public string SeatType { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Cols { get; set; }
        public double PricePerSeat { get; set; }
    }

    public class EventModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<EventLocationDTO> Locations { get; set; } = new();
        public List<DateTime> AvailableDates { get; set; } = new();
        public double OrganizingFee { get; set; }
        public List<SeatMatrixDTO> SeatMatrices { get; set; } = new();
        public List<string>? Images { get; set; }
        public bool IsPublished { get; set; }
        public bool IsApproved { get; set; }
    }
}
