namespace backend.DTOs
{
    public class RegisterRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "Customer";
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
    }
}
