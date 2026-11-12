namespace Ferreteria.Models.Segurudad
{
    public class Rol
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Usuario> Usuarios { get; set; } = [];
        public ICollection<RolPermiso> RolPermisos { get; set; } = [];

    }
}
