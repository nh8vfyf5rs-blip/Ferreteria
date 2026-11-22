using Ferreteria.Data;
using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Services
{
    public class CompraService(
        ICompraRepository repo,
        IProveedorRepository repoProveedor,
        IEmpleadoRepository repoEmpleado,
        IAlmacenRepository repoAlmacen,
        IProductoRepository repoProducto,
        IInventarioRepository repoInventario,
        IDetalleInventarioRepository repoDetalle,
        AppDbContext context) : ICompraService
    {
        public async Task<IReadOnlyList<CompraTableDto>> GetAllAsync()
        {
            var compras = await repo.GetAllAsync();

            return compras.Select(c => new CompraTableDto
            {
                Id = c.Id,
                FechaCompra = c.FechaCompra,
                Total = c.Total,
                ProveedorId = c.ProveedorId,
                ProveedorNombre = c.Proveedor?.NombreEmpresa ?? string.Empty,
                EmpleadoId = c.EmpleadoId,
                EmpleadoNombre = $"{c.Empleado?.Nombres} {c.Empleado?.Apellidos}".Trim(),
                AlmacenNombre = c.Almacen?.Name ?? string.Empty,
                SucursalNombre = c.Almacen?.Sucursal?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<CompraDto?> GetByIdAsync(int id)
        {
            var compra = await repo.GetByIdAsync(id);

            if (compra == null)
                return null;

            return new CompraDto
            {
                Id = compra.Id,
                FechaCompra = compra.FechaCompra,
                Total = compra.Total,
                ProveedorId = compra.ProveedorId,
                EmpleadoId = compra.EmpleadoId,
                AlmacenId = compra.AlmacenId,
                DetalleCompraDtos = compra.DetalleCompras.Select(d => new DetalleCompraDto
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.CantidadRecibida,
                    PrecioUnitario = d.CostoUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateCompraDto dto)
        {
            if (await repoProveedor.GetByIdAsync(dto.ProveedorId) == null)
                return (false, "El proveedor seleccionado no existe");

            if (await repoEmpleado.GetByIdAsync(dto.EmpleadoId) == null)
                return (false, "El empleado seleccionado no existe");

            var almacen = await context.Almacenes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe");

            if (almacen.SucursalId <= 0)
                return (false, "El almacén seleccionado no tiene una sucursal válida asignada");

            if (dto.DetalleCompraDtos == null || dto.DetalleCompraDtos.Count == 0)
                return (false, "Debe agregar al menos un producto a la compra");

            foreach (var detalle in dto.DetalleCompraDtos)
            {
                if (await repoProducto.GetByIdAsync(detalle.ProductoId) == null)
                    return (false, $"El producto con ID {detalle.ProductoId} no existe");

                if (detalle.Cantidad <= 0)
                    return (false, "La cantidad de cada producto debe ser mayor que cero");

                if (detalle.PrecioUnitario <= 0)
                    return (false, "El precio unitario debe ser mayor que cero");
            }

            var compra = new Compra
            {
                FechaCompra = dto.FechaCompra,
                ProveedorId = dto.ProveedorId,
                EmpleadoId = dto.EmpleadoId,
                AlmacenId = dto.AlmacenId,
                DetalleCompras = dto.DetalleCompraDtos.Select(d => new DetalleCompra
                {
                    ProductoId = d.ProductoId,
                    CantidadRecibida = d.Cantidad,
                    CostoUnitario = d.PrecioUnitario,
                    Subtotal = d.Cantidad * d.PrecioUnitario
                }).ToList()
            };

            compra.Total = compra.DetalleCompras.Sum(d => d.Subtotal);

            var compraCreada = await repo.CreateAsync(compra);

            if (compraCreada == null)
                return (false, "No fue posible registrar la compra");

            var inventarios = (await repoInventario.GetAllAsync()).ToList();

            foreach (var detalle in compraCreada.DetalleCompras)
            {
                var inventario = inventarios.FirstOrDefault(i =>
                    i.ProductoId == detalle.ProductoId &&
                    i.AlmacenId == compraCreada.AlmacenId);

                int stockAnterior;
                int stockNuevo;

                if (inventario == null)
                {
                    stockAnterior = 0;
                    stockNuevo = detalle.CantidadRecibida;

                    inventario = await repoInventario.CreateAsync(new Inventario
                    {
                        SucursalId = almacen.SucursalId,
                        ProductoId = detalle.ProductoId,
                        AlmacenId = compraCreada.AlmacenId,
                        Stock = stockNuevo,
                        FechaActualizacion = DateTime.Now
                    });

                    if (inventario == null)
                        return (false, "La compra se registró, pero no fue posible crear el inventario");

                    inventarios.Add(inventario);
                }
                else
                {
                    stockAnterior = inventario.Stock;
                    stockNuevo = stockAnterior + detalle.CantidadRecibida;

                    if (inventario.SucursalId <= 0)
                        inventario.SucursalId = almacen.SucursalId;

                    inventario.Stock = stockNuevo;
                    inventario.FechaActualizacion = DateTime.Now;

                    await repoInventario.UpdateAsync(inventario);
                }

                await repoDetalle.CreateAsync(new DetalleInventario
                {
                    InventarioId = inventario.Id,
                    SucursalId = almacen.SucursalId,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockNuevo,
                    FechaRegistro = DateTime.Now,
                    Observacion = $"Entrada por compra #{compraCreada.Id}"
                });
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateCompraDto dto)
        {
            var compra = await repo.GetByIdAsync(id);

            if (compra == null)
                return (false, "Compra no encontrada");

            if (await repoProveedor.GetByIdAsync(dto.ProveedorId) == null)
                return (false, "El proveedor seleccionado no existe");

            if (await repoEmpleado.GetByIdAsync(dto.EmpleadoId) == null)
                return (false, "El empleado seleccionado no existe");

            var almacenAnterior = await context.Almacenes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == compra.AlmacenId);

            var almacenNuevo = await context.Almacenes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == dto.AlmacenId);

            if (almacenNuevo == null)
                return (false, "El almacén seleccionado no existe");

            if (almacenNuevo.SucursalId <= 0)
                return (false, "El almacén seleccionado no tiene una sucursal válida asignada");

            if (compra.AlmacenId != dto.AlmacenId)
            {
                var detallesAgrupados = compra.DetalleCompras
                    .GroupBy(d => d.ProductoId)
                    .Select(g => new
                    {
                        ProductoId = g.Key,
                        Cantidad = g.Sum(d => d.CantidadRecibida)
                    })
                    .ToList();

                var inventarios = (await repoInventario.GetAllAsync()).ToList();

                foreach (var detalle in detallesAgrupados)
                {
                    var inventarioAnterior = inventarios.FirstOrDefault(i =>
                        i.ProductoId == detalle.ProductoId &&
                        i.AlmacenId == compra.AlmacenId);

                    if (inventarioAnterior == null ||
                        inventarioAnterior.Stock < detalle.Cantidad)
                    {
                        return (false,
                            "No hay existencias suficientes en el almacén original para trasladar esta compra");
                    }
                }

                foreach (var detalle in detallesAgrupados)
                {
                    var inventarioAnterior = inventarios.First(i =>
                        i.ProductoId == detalle.ProductoId &&
                        i.AlmacenId == compra.AlmacenId);

                    int stockAnteriorOrigen = inventarioAnterior.Stock;
                    int stockNuevoOrigen = stockAnteriorOrigen - detalle.Cantidad;

                    inventarioAnterior.Stock = stockNuevoOrigen;
                    inventarioAnterior.FechaActualizacion = DateTime.Now;

                    await repoInventario.UpdateAsync(inventarioAnterior);

                    await repoDetalle.CreateAsync(new DetalleInventario
                    {
                        InventarioId = inventarioAnterior.Id,
                        SucursalId = almacenAnterior?.SucursalId ?? 0,
                        StockAnterior = stockAnteriorOrigen,
                        StockNuevo = stockNuevoOrigen,
                        FechaRegistro = DateTime.Now,
                        Observacion = $"Salida por cambio de almacén en compra #{compra.Id}"
                    });

                    var inventarioNuevo = inventarios.FirstOrDefault(i =>
                        i.ProductoId == detalle.ProductoId &&
                        i.AlmacenId == dto.AlmacenId);

                    int stockAnteriorDestino;
                    int stockNuevoDestino;

                    if (inventarioNuevo == null)
                    {
                        stockAnteriorDestino = 0;
                        stockNuevoDestino = detalle.Cantidad;

                        inventarioNuevo = await repoInventario.CreateAsync(new Inventario
                        {
                            SucursalId = almacenNuevo.SucursalId,
                            ProductoId = detalle.ProductoId,
                            AlmacenId = dto.AlmacenId,
                            Stock = stockNuevoDestino,
                            FechaActualizacion = DateTime.Now
                        });

                        if (inventarioNuevo == null)
                            return (false, "No fue posible crear el inventario en el nuevo almacén");

                        inventarios.Add(inventarioNuevo);
                    }
                    else
                    {
                        stockAnteriorDestino = inventarioNuevo.Stock;
                        stockNuevoDestino = stockAnteriorDestino + detalle.Cantidad;

                        if (inventarioNuevo.SucursalId <= 0)
                            inventarioNuevo.SucursalId = almacenNuevo.SucursalId;

                        inventarioNuevo.Stock = stockNuevoDestino;
                        inventarioNuevo.FechaActualizacion = DateTime.Now;

                        await repoInventario.UpdateAsync(inventarioNuevo);
                    }

                    await repoDetalle.CreateAsync(new DetalleInventario
                    {
                        InventarioId = inventarioNuevo.Id,
                        SucursalId = almacenNuevo.SucursalId,
                        StockAnterior = stockAnteriorDestino,
                        StockNuevo = stockNuevoDestino,
                        FechaRegistro = DateTime.Now,
                        Observacion = $"Entrada por cambio de almacén en compra #{compra.Id}"
                    });
                }
            }

            compra.ProveedorId = dto.ProveedorId;
            compra.EmpleadoId = dto.EmpleadoId;
            compra.AlmacenId = dto.AlmacenId;

            await repo.UpdateAsync(compra);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var compra = await repo.GetByIdAsync(id);

            if (compra == null)
                return (false, "Compra no encontrada");

            var almacen = await context.Almacenes
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == compra.AlmacenId);

            var detallesAgrupados = compra.DetalleCompras
                .GroupBy(d => d.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    Cantidad = g.Sum(d => d.CantidadRecibida)
                })
                .ToList();

            var inventarios = (await repoInventario.GetAllAsync()).ToList();

            foreach (var detalle in detallesAgrupados)
            {
                var inventario = inventarios.FirstOrDefault(i =>
                    i.ProductoId == detalle.ProductoId &&
                    i.AlmacenId == compra.AlmacenId);

                if (inventario == null || inventario.Stock < detalle.Cantidad)
                {
                    return (false,
                        $"No hay existencias suficientes para revertir la compra del producto con ID {detalle.ProductoId}");
                }
            }

            foreach (var detalle in detallesAgrupados)
            {
                var inventario = inventarios.First(i =>
                    i.ProductoId == detalle.ProductoId &&
                    i.AlmacenId == compra.AlmacenId);

                int stockAnterior = inventario.Stock;
                int stockNuevo = stockAnterior - detalle.Cantidad;

                inventario.Stock = stockNuevo;
                inventario.FechaActualizacion = DateTime.Now;

                await repoInventario.UpdateAsync(inventario);

                await repoDetalle.CreateAsync(new DetalleInventario
                {
                    InventarioId = inventario.Id,
                    SucursalId = almacen?.SucursalId ?? 0,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockNuevo,
                    FechaRegistro = DateTime.Now,
                    Observacion = $"Eliminación de compra #{compra.Id}"
                });
            }

            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "No fue posible eliminar la compra");
        }
    }
}