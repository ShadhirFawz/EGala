using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace backend.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("passwordHash")]
        public string? PasswordHash { get; set; } // Nullable because Google login users won’t have a password

        [BsonElement("role")]
        public string Role { get; set; } = "Customer"; // Default role

        [BsonElement("profileImage")]
        public string? ProfileImage { get; set; } // URL to image

        [BsonElement("bio")]
        public string? Bio { get; set; }

        [BsonElement("preferredCategories")]
        public List<string> PreferredCategories { get; set; } = new();

        [BsonElement("points")]
        public int Points { get; set; } = 0;

        [BsonElement("walletBalance")]
        public double WalletBalance { get; set; } = 0.0;

        [BsonElement("notificationEnabled")]
        public bool NotificationEnabled { get; set; } = true;

        [BsonElement("googleId")]
        public string? GoogleId { get; set; } // For Google login

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}
