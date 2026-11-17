using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class DevolucionesService(IDevolucionesRepository repo,IVentaRepository repoVenta) : IDevolucionesService
    {
        public async Task<IReadOnlyList<DevolucionTableDto>> GetAllAsync()
        {
            var devoluciones = await repo.GetAllAsync();

            return devoluciones.Select(d => new DevolucionTableDto
            {
                Id = d.Id,
                FechaDevolucion = d.FechaDevolucion,
                Motivo = d.Motivo,
                Total = d.Total,
                VentaId = d.VentaId,
                ClienteNombre = d.Venta.Cliente.Name
            }).ToList();
        }

        public async Task<DevolucionDto?> GetByIdAsync(int id)
        {
            var devolucion = await repo.GetByIdAsync(id);

            if (devolucion == null)
                return null;

            return new DevolucionDto
            {
                Id = devolucion.Id,
                FechaDevolucion = devolucion.FechaDevolucion,
                Motivo = devolucion.Motivo,
                Total = devolucion.Total,
                VentaId = devolucion.VentaId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateDevolucionDto dto)
        {
            var venta = await repoVenta.GetByIdAsync(dto.VentaId);

            if (venta == null)
                return (false, "La venta seleccionada no existe");

            if (string.IsNullOrWhiteSpace(dto.Motivo))
                return (false, "Debe ingresar el motivo de la devolución");

            var devoluciones = await repo.GetAllAsync();

            if (devoluciones.Any(d => d.VentaId == dto.VentaId))
                return (false, "Esta venta ya tiene una devolución registrada");

            var devolucion = new Devoluciones
            {
                FechaDevolucion = DateTime.Now,
                Motivo = dto.Motivo.Trim(),
                Total = venta.Total,
                VentaId = venta.Id
            };

            var creada = await repo.CreateAsync(devolucion);

            return creada != null
                ? (true, null)
                : (false, "No fue posible registrar la devolución");
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateDevolucionDto dto)
        {
            var devolucion = await repo.GetByIdAsync(id);

            if (devolucion == null)
                return (false, "Devolución no encontrada");

            var venta = await repoVenta.GetByIdAsync(dto.VentaId);

            if (venta == null)
                return (false, "La venta seleccionada no existe");

            if (string.IsNullOrWhiteSpace(dto.Motivo))
                return (false, "Debe ingresar el motivo de la devolución");

            if (devolucion.VentaId != dto.VentaId)
            {
                var devoluciones = await repo.GetAllAsync();

                if (devoluciones.Any(d =>
                    d.VentaId == dto.VentaId && d.Id != id))
                {
                    return (false,
                        "Esta venta ya tiene una devolución registrada");
                }
            }

            devolucion.VentaId = venta.Id;
            devolucion.Motivo = dto.Motivo.Trim();
            devolucion.Total = venta.Total;

            await repo.UpdateAsync(devolucion);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "Devolución no encontrada");
        }
    }
}
