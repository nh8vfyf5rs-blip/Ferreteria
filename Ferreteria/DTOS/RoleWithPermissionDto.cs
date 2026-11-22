using Ferreteria.DTOS;

namespace Ferreteria.DTOS
{
    public class RoleWithPermissionDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PermisoDto> Permisos { get; set; } = [];
    }
}


