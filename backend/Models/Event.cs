using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class TicketPackage
    {
        [BsonElement("name")]
        public string Name { get; set; } = null!; // e.g. "Silver", "Gold", "VIP"

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        [BsonElement("materials")]
        public List<string> Materials { get; set; } = new(); // e.g. ["Mic", "Speakers"]
    }

    public class Seat
    {
        [BsonElement("row")]
        public int Row { get; set; }

        [BsonElement("col")]
        public int Col { get; set; }

        [BsonElement("isBooked")]
        public bool IsBooked { get; set; } = false;
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

        [BsonElement("locations")]
        public List<string> Locations { get; set; } = new(); // multiple locations

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("ticketPackages")]
        public List<TicketPackage> TicketPackages { get; set; } = new(); // up to 3

        [BsonElement("seatLayout")]
        public List<Seat>? SeatLayout { get; set; } // optional

        [BsonElement("totalCapacity")]
        public int TotalCapacity { get; set; }

        [BsonElement("bookedCount")]
        public int BookedCount { get; set; } = 0;

        [BsonElement("organizerId")]
        public string OrganizerId { get; set; } = null!;

        [BsonElement("images")]
        public List<string> Images { get; set; } = new(); // URLs or file paths

        [BsonElement("isPublished")]
        public bool IsPublished { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public bool IsSoldOut => BookedCount >= TotalCapacity;
    }
}
