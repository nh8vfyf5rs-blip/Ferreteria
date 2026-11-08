namespace Ferreteria.Models.Segurudad
{
    public class UsuarioPermiso
    {
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; } = null!;
        public int PermissionId { get; set; }
        public virtual Permiso permiso { get; set; } = null!;
        public bool isGranted { get; set; }
    }
}
