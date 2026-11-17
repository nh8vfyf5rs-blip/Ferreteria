using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class AlmacenService(
        IAlmacenRepository repo,
        ISucursalRepository sucursalRepo) : IAlmacenService
    {
        public async Task<IReadOnlyList<AlmacenTableDto>> GetAllAsync()
        {
            var almacenes = await repo.GetAllAsync();

            return almacenes.Select(a => new AlmacenTableDto
            {
                Id = a.Id,
                Name = a.Name,
                SucursalId = a.SucursalId,
                SucursalName = a.Sucursal.Name
            }).ToList();
        }

        public async Task<UpdateAlmacenDto?> GetByIdAsync(int id)
        {
            var almacen = await repo.GetByIdAsync(id);

            if (almacen == null)
                return null;

            return new UpdateAlmacenDto
            {
                Id = almacen.Id,
                Name = almacen.Name,
                SucursalId = almacen.SucursalId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateAlmacenDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del almacén es obligatorio.");

            var sucursal = await sucursalRepo.GetByIdAsync(dto.SucursalId);

            if (sucursal == null)
                return (false, "La sucursal seleccionada no existe.");

            await repo.CreateAsync(new Almacen
            {
                Name = dto.Name.Trim(),
                SucursalId = dto.SucursalId
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            UpdateAlmacenDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "El identificador del almacén no es válido.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre del almacén es obligatorio.");

            var almacen = await repo.GetByIdAsync(dto.Id.Value);

            if (almacen == null)
                return (false, "Almacén no encontrado.");

            var sucursal = await sucursalRepo.GetByIdAsync(dto.SucursalId);

            if (sucursal == null)
                return (false, "La sucursal seleccionada no existe.");

            almacen.Name = dto.Name.Trim();
            almacen.SucursalId = dto.SucursalId;

            await repo.UpdateAsync(almacen);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var almacen = await repo.GetByIdAsync(id);

            if (almacen == null)
                return (false, "Almacén no encontrado.");

            var tieneInventarios = await repo.HasInventariosAsync(id);

            if (tieneInventarios)
                return (false, "No se puede eliminar el almacén porque tiene inventario asociado.");

            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "No fue posible eliminar el almacén.");
        }
    }
}
