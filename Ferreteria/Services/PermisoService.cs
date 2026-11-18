using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class PermisoService(IPermisoRepository repo) : IPermisoService
    {
        public async Task<IReadOnlyList<PermisoTableDto>> GetAllAsync()
        {
            var permisos = await repo.GetAllAsync();

            return permisos.Select(p => new PermisoTableDto
            {
                Id = p.Id,
                Name = p.Name,
                Descripcion = p.Descripcion
            }).ToList();
        }

        public async Task<PermisoDto?> GetByIdAsync(int id)
        {
            var permiso = await repo.GetByIdAsync(id);

            if (permiso == null)
                return null;

            return new PermisoDto
            {
                Id = permiso.Id,
                Name = permiso.Name,
                Description = permiso.Descripcion
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreatePermisoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del permiso es obligatorio.");

            string nombre = dto.Name.Trim();

            var permisos = await repo.GetAllAsync();

            if (permisos.Any(p =>
                p.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Ya existe un permiso con ese nombre.");
            }

            var permiso = new Permiso
            {
                Name = nombre,
                Descripcion = dto.Description?.Trim() ?? string.Empty
            };

            await repo.CreateAsync(permiso);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdatePermisoDto dto)
        {
            var permiso = await repo.GetByIdAsync(id);

            if (permiso == null)
                return (false, "El permiso no existe.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del permiso es obligatorio.");

            string nombre = dto.Name.Trim();

            var permisos = await repo.GetAllAsync();

            if (permisos.Any(p =>
                p.Id != id &&
                p.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Ya existe otro permiso con ese nombre.");
            }

            permiso.Name = nombre;
            permiso.Descripcion = dto.Description?.Trim() ?? string.Empty;

            await repo.UpdateAsync(permiso);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var permiso = await repo.GetByIdAsync(id);

            if (permiso == null)
                return (false, "El permiso no existe.");

            if (permiso.RolPermissions.Any())
                return (false, "No se puede eliminar el permiso porque está asociado a uno o más roles.");

            if (permiso.UsuarioPermisos.Any())
                return (false, "No se puede eliminar el permiso porque está asignado directamente a uno o más usuarios.");

            await repo.DeleteAsync(id);

            return (true, null);
        }
    }
}
