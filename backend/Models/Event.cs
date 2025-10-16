using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models
{
    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("category")]
        public string Category { get; set; } = null!;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("location")]
        public string Location { get; set; } = null!;

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("availableSeats")]
        public int AvailableSeats { get; set; }

        [BsonElement("organizerId")]
        public string OrganizerId { get; set; } = null!;

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; } = false;

        [BsonElement("isPublished")]
        public bool IsPublished { get; set; } = false;
    }
}
