namespace Ferreteria.DTOS
{
    public class UserPermisoOverrideDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
    }
}
