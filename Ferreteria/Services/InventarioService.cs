using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class InventarioService(
        IInventarioRepository repo,
        IAlmacenRepository repoAlmacen,
        IProductoRepository repoProducto,
        IDetalleInventarioRepository repoDetalle) : IInventarioService
    {
        public async Task<IReadOnlyList<InventarioTableDto>> GetAllAsync()
        {
            var inventarios = await repo.GetAllAsync();

            return inventarios.Select(i => new InventarioTableDto
            {
                Id = i.Id,
                SucursalId = i.Almacen?.SucursalId ?? i.SucursalId,
                SucursalNombre = i.Almacen?.Sucursal?.Name ?? string.Empty,
                AlmacenId = i.AlmacenId,
                AlmacenNombre = i.Almacen?.Name ?? string.Empty,
                ProductoId = i.ProductoId,
                ProductoNombre = i.Producto?.Nombre ?? string.Empty,
                Stock = i.Stock,
                FechaActualizacion = i.FechaActualizacion
            }).ToList();
        }

        public async Task<InventarioDto?> GetByIdAsync(int id)
        {
            var inventario = await repo.GetByIdAsync(id);

            if (inventario == null)
                return null;

            return new InventarioDto
            {
                Id = inventario.Id,
                SucursalId = inventario.Almacen?.SucursalId ?? inventario.SucursalId,
                AlmacenId = inventario.AlmacenId,
                ProductoId = inventario.ProductoId,
                Stock = inventario.Stock,
                FechaActualizacion = inventario.FechaActualizacion,
                DetalleInventarioDTO = inventario.DetalleInventarios
                    .Select(d => new DetalleInventarioDTO
                    {
                        Id = d.Id,
                        SucursalId = d.SucursalId,
                        StockAnterior = d.StockAnterior,
                        StockNuevo = d.StockNuevo,
                        FechaRegistro = d.FechaRegistro,
                        Observacion = d.Observacion,
                        InventarioId = d.InventarioId
                    }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateInventarioDTO dto)
        {
            if (dto.Stock < 0)
                return (false, "El stock inicial no puede ser negativo.");

            var almacen = await repoAlmacen.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

            if (almacen.SucursalId <= 0)
                return (false, "El almacén seleccionado no tiene una sucursal válida asignada.");

            if (await repoProducto.GetByIdAsync(dto.ProductoId) == null)
                return (false, "El producto seleccionado no existe.");

            var inventarios = await repo.GetAllAsync();

            if (inventarios.Any(i =>
                i.AlmacenId == dto.AlmacenId &&
                i.ProductoId == dto.ProductoId))
            {
                return (false, "Ya existe un inventario para este producto en el almacén seleccionado.");
            }

            var fechaActual = DateTime.Now;

            var inventario = new Inventario
            {
                SucursalId = almacen.SucursalId,
                AlmacenId = dto.AlmacenId,
                ProductoId = dto.ProductoId,
                Stock = dto.Stock,
                FechaActualizacion = fechaActual,
                DetalleInventarios = new List<DetalleInventario>
                {
                    new()
                    {
                        SucursalId = almacen.SucursalId,
                        StockAnterior = 0,
                        StockNuevo = dto.Stock,
                        FechaRegistro = fechaActual,
                        Observacion = "Registro inicial de inventario"
                    }
                }
            };

            var creado = await repo.CreateAsync(inventario);

            if (creado == null)
                return (false, "No fue posible crear el inventario.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateInventarioDTO dto)
        {
            if (dto.Stock < 0)
                return (false, "El stock no puede ser negativo.");

            var inventario = await repo.GetByIdAsync(id);

            if (inventario == null)
                return (false, "El inventario no existe.");

            var almacen = await repoAlmacen.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

            if (almacen.SucursalId <= 0)
                return (false, "El almacén seleccionado no tiene una sucursal válida asignada.");

            if (await repoProducto.GetByIdAsync(dto.ProductoId) == null)
                return (false, "El producto seleccionado no existe.");

            var inventarios = await repo.GetAllAsync();

            if (inventarios.Any(i =>
                i.Id != id &&
                i.AlmacenId == dto.AlmacenId &&
                i.ProductoId == dto.ProductoId))
            {
                return (false, "Ya existe un inventario para este producto en el almacén seleccionado.");
            }

            int stockAnterior = inventario.Stock;
            int stockNuevo = dto.Stock;

            bool cambioUbicacion =
                inventario.AlmacenId != dto.AlmacenId ||
                inventario.ProductoId != dto.ProductoId;

            inventario.SucursalId = almacen.SucursalId;
            inventario.AlmacenId = dto.AlmacenId;
            inventario.ProductoId = dto.ProductoId;
            inventario.Stock = stockNuevo;
            inventario.FechaActualizacion = DateTime.Now;
            inventario.DetalleInventarios ??= new List<DetalleInventario>();

            if (stockAnterior != stockNuevo || cambioUbicacion)
            {
                inventario.DetalleInventarios.Add(new DetalleInventario
                {
                    SucursalId = almacen.SucursalId,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockNuevo,
                    FechaRegistro = DateTime.Now,
                    Observacion = cambioUbicacion
                        ? "Actualización de inventario y ubicación"
                        : "Ajuste de inventario"
                });
            }

            var actualizado = await repo.UpdateAsync(inventario);

            if (!actualizado)
                return (false, "No fue posible actualizar el inventario.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var inventario = await repo.GetByIdAsync(id);

            if (inventario == null)
                return (false, "El inventario no existe.");

            if (inventario.Stock > 0)
                return (false, "No se puede eliminar un inventario que todavía tiene existencias.");

            if (inventario.DetalleInventarios.Any())
                return (false, "No se puede eliminar el inventario porque tiene un historial de movimientos de stock.");

            var detalles = await repoDetalle.GetAllAsync();

            if (detalles.Any(d => d.InventarioId == id))
                return (false, "No se puede eliminar el inventario porque tiene detalles de inventario asociados.");

            var eliminado = await repo.DeleteAsync(id);

            return eliminado
                ? (true, null)
                : (false, "No fue posible eliminar el inventario.");
        }
    }
}