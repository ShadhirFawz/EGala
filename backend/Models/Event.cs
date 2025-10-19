using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class EventLocation
    {
        [BsonElement("name")]
        public string Name { get; set; } = null!; // e.g. "Colombo Stadium"

        [BsonElement("coordinates")]
        public GeoJsonPoint<GeoJson2DCoordinates> Coordinates { get; set; } = null!;
    }

    public class SeatMatrix
    {
        [BsonElement("seatType")]
        public string SeatType { get; set; } = "Normal"; // "Normal" or "Special"

        [BsonElement("rows")]
        public int Rows { get; set; }

        [BsonElement("cols")]
        public int Cols { get; set; }

        [BsonElement("pricePerSeat")]
        public double PricePerSeat { get; set; } // 0, 5, or 10
    }

    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!; // supports rich text HTML

        [BsonElement("category")]
        public string Category { get; set; } = null!;

        // Multiple named locations with coordinates
        [BsonElement("locations")]
        public List<EventLocation> Locations { get; set; } = new();

        // Multiple available dates (organizer chooses possible dates)
        [BsonElement("availableDates")]
        public List<DateTime> AvailableDates { get; set; } = new();

        // Fixed organizing fee shown initially on event page
        [BsonElement("organizingFee")]
        public double OrganizingFee { get; set; } = 0.0;

        // Seat definitions (matrices) for Normal and Special seats
        [BsonElement("seatMatrices")]
        public List<SeatMatrix> SeatMatrices { get; set; } = new();

        [BsonElement("images")]
        public List<string> Images { get; set; } = new(); // frontend uploads & sends URLs

        [BsonElement("organizerId")]
        public string OrganizerId { get; set; } = null!;

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; } = false;

        // Event must be approved before publishing — creation sets false
        [BsonElement("isPublished")]
        public bool IsPublished { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // calculated property — not saved
        [BsonIgnore]
        public bool IsSoldOut { get; set; } = false;
    }
}
