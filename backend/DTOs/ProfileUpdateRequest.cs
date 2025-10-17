namespace backend.DTOs
{
    public class ProfileUpdateRequest
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public bool? NotificationEnabled { get; set; }
        public List<string>? PreferredCategories { get; set; }
    }
}
