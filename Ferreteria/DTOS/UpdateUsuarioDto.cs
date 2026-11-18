namespace Ferreteria.DTOS
{
    public class UpdateUsuarioDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}
