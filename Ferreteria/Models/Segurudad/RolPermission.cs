using Ferreteria.Models.Segurudad;

namespace Ferreteria.Models
{
    public class RolPermission
    {
        public int RolId { get; set; }
        public virtual Rol Rol { get; set; } = null;
        public int PermissionId { get; set; }
        public virtual Permiso Permission { get; set; } = null!;

        public bool isGranted { get; set; }
    }
}
