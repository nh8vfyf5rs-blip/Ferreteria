using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class DetalleInventarioService(
        IDetalleInventarioRepository repo,
        IInventarioRepository repoInventario) : IDetalleInventarioService
    {
        public async Task<IReadOnlyList<DetalleInventarioTableDto>> GetAllAsync()
        {
            var detalles = await repo.GetAllAsync(); 

            return detalles.Select(d => new DetalleInventarioTableDto
            {
                Id = d.Id,
                SucursalNombre = d.Sucursal?.Name ?? d.Inventario?.Almacen?.Sucursal?.Name ?? string.Empty,
                AlmacenNombre = d.Inventario?.Almacen?.Name ?? string.Empty,
                ProductoNombre = d.Inventario?.Producto?.Nombre ?? string.Empty,
                StockAnterior = d.StockAnterior,
                StockNuevo = d.StockNuevo,
                FechaRegistro = d.FechaRegistro,
                Observacion = d.Observacion
            }).ToList();
        }

        public async Task<DetalleInventarioDTO?> GetByIdAsync(int id)
        {
            var detalle = await repo.GetByIdAsync(id);

            if (detalle == null)
                return null;

            return new DetalleInventarioDTO
            {
                Id = detalle.Id,
                InventarioId = detalle.InventarioId,
                SucursalId = detalle.SucursalId,
                StockAnterior = detalle.StockAnterior,
                StockNuevo = detalle.StockNuevo,
                FechaRegistro = detalle.FechaRegistro,
                Observacion = detalle.Observacion
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateDetalleInventarioDto dto)
        {
            if (dto.StockAnterior < 0 || dto.StockNuevo < 0)
                return (false, "Las existencias no pueden ser negativas.");

            var inventario = await repoInventario.GetByIdAsync(dto.InventarioId);

            if (inventario == null)
                return (false, "El inventario seleccionado no existe.");

            int sucursalId = dto.SucursalId > 0
                ? dto.SucursalId
                : inventario.Almacen?.SucursalId ?? 0;

            if (sucursalId <= 0)
                return (false, "La sucursal no es válida o no está asociada al inventario.");

            var detalle = new DetalleInventario
            {
                InventarioId = dto.InventarioId,
                SucursalId = sucursalId,
                StockAnterior = dto.StockAnterior,
                StockNuevo = dto.StockNuevo,
                FechaRegistro = DateTime.Now,
                Observacion = dto.Observacion?.Trim() ?? string.Empty
            };

            await repo.CreateAsync(detalle);

            inventario.Stock = dto.StockNuevo;
            inventario.FechaActualizacion = DateTime.Now;
            await repoInventario.UpdateAsync(inventario);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateDetalleInventarioDto dto)
        {
            var detalle = await repo.GetByIdAsync(id);

            if (detalle == null)
                return (false, "El detalle de inventario no existe.");

            if (dto.StockAnterior < 0 || dto.StockNuevo < 0)
                return (false, "Las existencias no pueden ser negativas.");

            var inventario = await repoInventario.GetByIdAsync(dto.InventarioId);

            if (inventario == null)
                return (false, "El inventario seleccionado no existe.");

            int sucursalId = dto.SucursalId > 0
                ? dto.SucursalId
                : inventario.Almacen?.SucursalId ?? detalle.SucursalId;

            if (sucursalId <= 0)
                return (false, "La sucursal no es válida.");

            detalle.InventarioId = dto.InventarioId;
            detalle.SucursalId = sucursalId;
            detalle.StockAnterior = dto.StockAnterior;
            detalle.StockNuevo = dto.StockNuevo;
            detalle.Observacion = dto.Observacion?.Trim() ?? string.Empty;

            await repo.UpdateAsync(detalle);

            inventario.Stock = dto.StockNuevo;
            inventario.FechaActualizacion = DateTime.Now;
            await repoInventario.UpdateAsync(inventario);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var detalle = await repo.GetByIdAsync(id);

            if (detalle == null)
                return (false, "El detalle de inventario no existe.");

            await repo.DeleteAsync(id);
            return (true, null);
        }
    }
}