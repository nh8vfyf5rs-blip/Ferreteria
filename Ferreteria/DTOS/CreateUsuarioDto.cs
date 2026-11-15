namespace Ferreteria.DTOS
{
    public class CreateUsuarioDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}
