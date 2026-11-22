using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class MetodoPagoService(IMetodoPagoRepository repo) : IMetodoPagoService
    {
        public async Task<IReadOnlyList<MetodoPagoTableDto>> GetAllAsync()
        {
            var metodosPago = await repo.GetAllAsync();

            return metodosPago.Select(m => new MetodoPagoTableDto
            {
                Id = m.Id,
                Name = m.Name,
                CantidadPagos = m.DetallePagos?.Count ?? 0,
                TotalPagos = m.DetallePagos?.Sum(dp => dp.Monto) ?? 0m
            }).ToList();
        }

        public async Task<MetodoPagoDto?> GetByIdAsync(int id)
        {
            var metodoPago = await repo.GetByIdAsync(id);

            if (metodoPago == null)
                return null;

            return new MetodoPagoDto
            {
                Id = metodoPago.Id,
                Nombre = metodoPago.Name
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateMetodoPagoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (false, "El nombre del método de pago es obligatorio.");

            var metodoPago = new MetodoPago
            {
                Name = dto.Nombre.Trim()
            };

            await repo.CreateAsync(metodoPago);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateMetodoPagoDto dto)
        {
            var metodoPago = await repo.GetByIdAsync(id);

            if (metodoPago == null)
                return (false, "El método de pago no existe.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (false, "El nombre del método de pago es obligatorio.");

            metodoPago.Name = dto.Nombre.Trim();

            await repo.UpdateAsync(metodoPago);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var metodoPago = await repo.GetByIdAsync(id);

            if (metodoPago == null)
                return (false, "El método de pago no existe.");

            if (metodoPago.DetallePagos != null && metodoPago.DetallePagos.Any())
                return (false, "No se puede eliminar el método de pago porque tiene detalles de pago asociados.");

            await repo.DeleteAsync(id);

            return (true, null);
        }
    }
}