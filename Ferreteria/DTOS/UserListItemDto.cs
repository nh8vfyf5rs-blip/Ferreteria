namespace Ferreteria.DTOS
{
    public class UserListItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RolName { get; set; } = string.Empty;
    }
}
