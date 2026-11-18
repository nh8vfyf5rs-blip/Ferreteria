using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class SucursalService(
        ISucursalRepository sucursalRepository) : ISucursalService
    {
        public async Task<IReadOnlyList<SucursalTableDto>> GetAllAsync()
        {
            var sucursales = await sucursalRepository.GetAllAsync();

            return sucursales.Select(s => new SucursalTableDto
            {
                Id = s.Id,
                Name = s.Name,
                Direccion = s.Direccion,
                Telefono = s.Telefono
            }).ToList();
        }

        public async Task<SucursalDto?> GetByIdAsync(int id)
        {
            var sucursal = await sucursalRepository.GetByIdAsync(id);

            if (sucursal == null)
                return null;

            return new SucursalDto
            {
                Id = sucursal.Id,
                Name = sucursal.Name,
                Direccion = sucursal.Direccion,
                Telefono = sucursal.Telefono
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateSucursalDto dto)
        {
            var nombre = dto.Name.Trim();

            var sucursales = await sucursalRepository.GetAllAsync();

            if (sucursales.Any(s =>
                s.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe una sucursal con ese nombre");

            var sucursal = new Sucursal
            {
                Name = nombre,
                Direccion = dto.Direccion.Trim(),
                Telefono = dto.Telefono.Trim()
            };

            var created = await sucursalRepository.CreateAsync(sucursal);

            if (created == null)
                return (false, "No se pudo crear la sucursal");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id,
            UpdateSucursalDto dto)
        {
            var sucursal = await sucursalRepository.GetByIdAsync(id);

            if (sucursal == null)
                return (false, "Sucursal no encontrada");

            var nombre = dto.Name.Trim();

            var sucursales = await sucursalRepository.GetAllAsync();

            if (sucursales.Any(s =>
                s.Id != id &&
                s.Name.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe otra sucursal con ese nombre");

            sucursal.Name = nombre;
            sucursal.Direccion = dto.Direccion.Trim();
            sucursal.Telefono = dto.Telefono.Trim();

            var updated = await sucursalRepository.UpdateAsync(sucursal);

            if (!updated)
                return (false, "No se pudo actualizar la sucursal");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var sucursal = await sucursalRepository.GetByIdAsync(id);

            if (sucursal == null)
                return (false, "Sucursal no encontrada");

            if (sucursal.Almacenes.Any())
                return (false, "No se puede eliminar la sucursal porque tiene almacenes asociados");

            if (sucursal.Empleados.Any())
                return (false, "No se puede eliminar la sucursal porque tiene empleados asociados");

            var deleted = await sucursalRepository.DeleteAsync(id);

            if (!deleted)
                return (false, "No se pudo eliminar la sucursal");

            return (true, null);
        }
    }
}
