using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class ProveedorService(IProveedorRepository proveedorRepository) : IProveedorService
    {
        public async Task<IReadOnlyList<ProveedorTableDto>> GetAllAsync()
        {
            var proveedores = await proveedorRepository.GetAllAsync();

            return proveedores.Select(proveedor => new ProveedorTableDto
            {
                Id = proveedor.Id,
                NombreEmpresa = proveedor.NombreEmpresa,
                Contacto = proveedor.Contacto,
                Correo = proveedor.Correo,
                Direccion = proveedor.Direccion,
                FechaRegistro = proveedor.FechaRegistro
            }).ToList();
        }

        public async Task<ProveedorDto?> GetByIdAsync(int id)
        {
            var proveedor = await proveedorRepository.GetByIdAsync(id);

            if (proveedor == null)
                return null;

            return new ProveedorDto
            {
                Id = proveedor.Id,
                NombreEmpresa = proveedor.NombreEmpresa,
                Contacto = proveedor.Contacto,
                Correo = proveedor.Correo,
                Direccion = proveedor.Direccion,
                FechaRegistro = proveedor.FechaRegistro
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateProveedorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreEmpresa))
                return (false, "El nombre de la empresa es obligatorio");

            var proveedores = await proveedorRepository.GetAllAsync();

            if (proveedores.Any(p =>
                p.NombreEmpresa.Equals(dto.NombreEmpresa.Trim(), StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe un proveedor con ese nombre de empresa");

            var proveedor = new Proveedor
            {
                NombreEmpresa = dto.NombreEmpresa.Trim(),
                Contacto = dto.Contacto.Trim(),
                Correo = dto.Correo.Trim(),
                Direccion = dto.Direccion.Trim(),
                FechaRegistro = DateTime.Now
            };

            var resultado = await proveedorRepository.CreateAsync(proveedor);

            if (resultado == null)
                return (false, "No fue posible crear el proveedor");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateProveedorDto dto)
        {
            var proveedor = await proveedorRepository.GetByIdAsync(id);

            if (proveedor == null)
                return (false, "Proveedor no encontrado");

            if (string.IsNullOrWhiteSpace(dto.NombreEmpresa))
                return (false, "El nombre de la empresa es obligatorio");

            var proveedores = await proveedorRepository.GetAllAsync();

            if (proveedores.Any(p =>
                p.Id != id &&
                p.NombreEmpresa.Equals(dto.NombreEmpresa.Trim(), StringComparison.OrdinalIgnoreCase)))
                return (false, "Ya existe otro proveedor con ese nombre de empresa");

            proveedor.NombreEmpresa = dto.NombreEmpresa.Trim();
            proveedor.Contacto = dto.Contacto.Trim();
            proveedor.Correo = dto.Correo.Trim();
            proveedor.Direccion = dto.Direccion.Trim();

            var resultado = await proveedorRepository.UpdateAsync(proveedor);

            if (!resultado)
                return (false, "No fue posible actualizar el proveedor");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var proveedor = await proveedorRepository.GetByIdAsync(id);

            if (proveedor == null)
                return (false, "Proveedor no encontrado");

            if (proveedor.Compras.Any())
                return (false, "No se puede eliminar el proveedor porque tiene compras asociadas");

            var resultado = await proveedorRepository.DeleteAsync(id);

            if (!resultado)
                return (false, "No fue posible eliminar el proveedor");

            return (true, null);
        }
    }
}
