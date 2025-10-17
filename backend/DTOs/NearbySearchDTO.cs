namespace backend.DTOs
{
    public class NearbySearchDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Distance in kilometers
        public double RadiusKm { get; set; } = 10;

        // Optional filters
        public string? Category { get; set; }
        public string? Keyword { get; set; }
    }
}
