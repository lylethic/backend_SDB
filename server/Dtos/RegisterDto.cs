namespace server.Dtos
{
    public class RegisterDto
    {
        public int RoleId { get; set; }

        public int? SchoolId { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
