using Ferreteria.Models.Segurudad;

namespace Ferreteria.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public string Name { get; set; }=string.Empty;
        public string Descripcion {  get; set; }=string.Empty;
        public virtual ICollection<RolPermiso> RolPermissions { get; set; } = [];
        public virtual ICollection<UsuarioPermiso> UsuarioPermisos { get;  set; } = [];
    }
}
