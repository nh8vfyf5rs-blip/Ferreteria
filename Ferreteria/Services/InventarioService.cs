using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class InventarioService(IInventarioRepository repo,
        IAlmacenRepository repoAlmacen,
        IProductoRepository repoProducto,
        IMovimientoInventarioRepository repoMovimiento) : IInventarioService
    {
        public async Task<IReadOnlyList<InventarioTableDto>> GetAllAsync()
        {
            var inventarios = await repo.GetAllAsync();

            return inventarios.Select(i => new InventarioTableDto
            {
                Id = i.Id,
                SucursalId = i.Almacen.SucursalId,
                SucursalNombre = i.Almacen.Sucursal.Name,
                AlmacenId = i.AlmacenId,
                AlmacenNombre = i.Almacen.Name,
                ProductoId = i.ProductoId,
                ProductoNombre = i.Producto.Nombre,
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
                SucursalId = inventario.Almacen.SucursalId,
                AlmacenId = inventario.AlmacenId,
                ProductoId = inventario.ProductoId,
                Stock = inventario.Stock,
                FechaActualizacion = inventario.FechaActualizacion,
                detalleInventarioDTOs = inventario.DetalleInventarios
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

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateInventarioDTO dto)
        {
            var almacen = await repoAlmacen.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

            if (await repoProducto.GetByIdAsync(dto.ProductoId) == null)
                return (false, "El producto seleccionado no existe.");

            var inventarios = await repo.GetAllAsync();

            if (inventarios.Any(i =>
                i.AlmacenId == dto.AlmacenId &&
                i.ProductoId == dto.ProductoId))
            {
                return (false, "Ya existe un inventario para este producto en el almacén seleccionado.");
            }

            var inventario = new Inventario
            {
                AlmacenId = dto.AlmacenId,
                ProductoId = dto.ProductoId,
                Stock = dto.Stock,
                FechaActualizacion = DateTime.Now
            };

            inventario.DetalleInventarios.Add(new DetalleInventario
            {
                SucursalId = almacen.SucursalId,
                StockAnterior = 0,
                StockNuevo = dto.Stock,
                FechaRegistro = DateTime.Now,
                Observacion = "Registro inicial de inventario"
            });

            await repo.CreateAsync(inventario);

            if (dto.Stock > 0)
            {
                await repoMovimiento.CreateAsync(new MovimientoInventario
                {
                    TipoMovimiento = "Entrada",
                    Cantidad = dto.Stock,
                    Observacion = "Registro inicial de inventario",
                    InventarioId = inventario.Id
                });
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateInventarioDTO dto)
        {
            var inventario = await repo.GetByIdAsync(id);

            if (inventario == null)
                return (false, "El inventario no existe.");

            var almacen = await repoAlmacen.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

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
            int diferencia = Math.Abs(stockNuevo - stockAnterior);

            bool cambioUbicacion =
                inventario.AlmacenId != dto.AlmacenId ||
                inventario.ProductoId != dto.ProductoId;

            inventario.AlmacenId = dto.AlmacenId;
            inventario.ProductoId = dto.ProductoId;
            inventario.Stock = stockNuevo;
            inventario.FechaActualizacion = DateTime.Now;

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

            await repo.UpdateAsync(inventario);

            if (diferencia > 0)
            {
                await repoMovimiento.CreateAsync(new MovimientoInventario
                {
                    TipoMovimiento = stockNuevo > stockAnterior
                        ? "Entrada"
                        : "Salida",
                    Cantidad = diferencia,
                    Observacion = "Ajuste de inventario",
                    InventarioId = inventario.Id
                });
            }

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

            var movimientos = await repoMovimiento.GetAllAsyc();

            if (movimientos.Any(m => m.InventarioId == id))
                return (false, "No se puede eliminar el inventario porque tiene movimientos asociados.");

            await repo.DeleteAsync(id);

            return (true, null);
        }
    }
}
