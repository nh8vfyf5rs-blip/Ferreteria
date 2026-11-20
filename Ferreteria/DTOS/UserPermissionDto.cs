namespace Ferreteria.DTOS
{
    public class UserPermissionDto
    {
  public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public List<string> EffectivePermission { get; set; } = [];
    public List<UserPermisoOverrideDto> DirectPermission { get; set; } = [];
    public string RolName { get; internal set; }
}
}
