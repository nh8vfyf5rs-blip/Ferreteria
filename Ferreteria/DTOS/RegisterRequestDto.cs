namespace Ferreteria.DTOS
{
    public class RegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public int RolId { get; set; } = 2;
    }
}
