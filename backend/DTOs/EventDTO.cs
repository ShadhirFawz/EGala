using System;
using System.Collections.Generic;

namespace backend.DTOs
{
    public class EventLocationDTO
    {
        public string Name { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class SeatMatrixDTO
    {
        public string SeatType { get; set; } = "Normal"; // "Normal" or "Special"
        public int Rows { get; set; }
        public int Cols { get; set; }
        public double PricePerSeat { get; set; } // 0, 5, or 10
    }

    public class EventDTO
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public List<EventLocationDTO> Locations { get; set; } = new();
        public List<DateTime> AvailableDates { get; set; } = new();
        public double OrganizingFee { get; set; } = 0.0;
        public List<SeatMatrixDTO> SeatMatrices { get; set; } = new();
        public List<string>? Images { get; set; }
        // Note: organizer should not provide ticket packages — those are chosen by customer during booking
    }
}
