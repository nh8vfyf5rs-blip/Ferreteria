namespace Ferreteria.DTOS
{
    public class UserPermissionsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<string> EffectivePermission { get; set; } = [];
        public List<UserPermisoOverrideDto> DirectPermiso { get; set; } = [];
    }
}
