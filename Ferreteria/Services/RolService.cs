using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Models.Segurudad;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class RolService(
        IRolRepository rolRepository,
        IPermisoRepository permisoRepository) : IRolService
    {
        public async Task<IReadOnlyList<RolTableDto>> GetAllAsync()
        {
            var roles = await rolRepository.GetAllAsync();

            return roles.Select(r => new RolTableDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }

        public async Task<RolDto?> GetByIdAsync(int id)
        {
            var rol = await rolRepository.GetByIdAsync(id);

            if (rol == null)
                return null;

            return new RolDto
            {
                Id = rol.Id,
                Name = rol.Name,
                PermisoIds = rol.RolPermisos
                    .Where(rp => rp.IsGranted)
                    .Select(rp => rp.PermisoId)
                    .ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateRolDto dto)
        {
            var nombre = dto.Name.Trim();

            var roles = await rolRepository.GetAllAsync();

            if (roles.Any(r =>
                r.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe un rol con ese nombre");

            var permisos = new List<RolPermiso>();

            foreach (var permisoId in dto.PermisoId ?? [])
            {
                var permiso = await permisoRepository.GetByIdAsync(permisoId);

                if (permiso == null)
                    return (false, $"El permiso con Id {permisoId} no existe");

                permisos.Add(new RolPermiso
                {
                    PermisoId = permisoId,
                    IsGranted = true
                });
            }

            await rolRepository.CreateAsync(new Rol
            {
                Name = nombre,
                RolPermisos = permisos
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id,
            UpdateRolDto dto)
        {
            var rol = await rolRepository.GetByIdAsync(id);

            if (rol == null)
                return (false, "Rol no encontrado");

            var nombre = dto.Name.Trim();

            var roles = await rolRepository.GetAllAsync();

            if (roles.Any(r =>
                r.Id != id &&
                r.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe otro rol con ese nombre");

            rol.Name = nombre;

            await rolRepository.UpdateAsync(rol);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var rol = await rolRepository.GetByIdAsync(id);

            if (rol == null)
                return (false, "Rol no encontrado");

            if (rol.Usuarios.Any())
                return (false, "No se puede eliminar el rol porque tiene usuarios asociados");

            var deleted = await rolRepository.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "No se pudo eliminar el rol");
        }
    }
}
