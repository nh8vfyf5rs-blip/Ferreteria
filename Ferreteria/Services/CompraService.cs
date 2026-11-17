using Ferreteria.Data;
using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class CompraService(
        ICompraRepository repo,
        IProveedorRepository repoProveedor,
        IEmpleadoRepository repoEmpleado,
        IAlmacenRepository repoAlmacen,
        IProductoRepository repoProducto,
        IInventarioRepository repoInventario,
        IMovimientoInventarioRepository repoMovimiento) : ICompraService
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
                ProveedorNombre = c.Proveedor.NombreEmpresa,
                EmpleadoId = c.EmpleadoId,
                EmpleadoNombre = $"{c.Empleado.Nombres} {c.Empleado.Apellidos}"
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

            if (await repoAlmacen.GetByIdAsync(dto.AlmacenId) == null)
                return (false, "El almacén seleccionado no existe");

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

            foreach (var detalle in compraCreada.DetalleCompras)
            {
                var inventarios = await repoInventario.GetAllAsync();

                var inventario = inventarios.FirstOrDefault(i =>
                    i.ProductoId == detalle.ProductoId &&
                    i.AlmacenId == compraCreada.AlmacenId);

                if (inventario == null)
                {
                    inventario = await repoInventario.CreateAsync(new Inventario
                    {
                        ProductoId = detalle.ProductoId,
                        AlmacenId = compraCreada.AlmacenId,
                        Stock = detalle.CantidadRecibida,
                        FechaActualizacion = DateTime.Now
                    });

                    if (inventario == null)
                        return (false, "La compra se registró, pero no fue posible crear el inventario");
                }
                else
                {
                    inventario.Stock += detalle.CantidadRecibida;
                    inventario.FechaActualizacion = DateTime.Now;

                    await repoInventario.UpdateAsync(inventario);
                }

                await repoMovimiento.CreateAsync(new MovimientoInventario
                {
                    TipoMovimiento = "Entrada por compra",
                    Cantidad = detalle.CantidadRecibida,
                    Fecha = DateTime.Now,
                    Observacion = $"Compra #{compraCreada.Id}",
                    InventarioId = inventario.Id
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

            if (await repoAlmacen.GetByIdAsync(dto.AlmacenId) == null)
                return (false, "El almacén seleccionado no existe");

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

                var inventarios = await repoInventario.GetAllAsync();

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

                    inventarioAnterior.Stock -= detalle.Cantidad;
                    inventarioAnterior.FechaActualizacion = DateTime.Now;

                    await repoInventario.UpdateAsync(inventarioAnterior);

                    await repoMovimiento.CreateAsync(new MovimientoInventario
                    {
                        TipoMovimiento = "Salida por cambio de almacén",
                        Cantidad = detalle.Cantidad,
                        Fecha = DateTime.Now,
                        Observacion = $"Compra #{compra.Id}",
                        InventarioId = inventarioAnterior.Id
                    });

                    var inventarioNuevo = inventarios.FirstOrDefault(i =>
                        i.ProductoId == detalle.ProductoId &&
                        i.AlmacenId == dto.AlmacenId);

                    if (inventarioNuevo == null)
                    {
                        inventarioNuevo = await repoInventario.CreateAsync(new Inventario
                        {
                            ProductoId = detalle.ProductoId,
                            AlmacenId = dto.AlmacenId,
                            Stock = detalle.Cantidad,
                            FechaActualizacion = DateTime.Now
                        });

                        if (inventarioNuevo == null)
                            return (false, "No fue posible crear el inventario en el nuevo almacén");
                    }
                    else
                    {
                        inventarioNuevo.Stock += detalle.Cantidad;
                        inventarioNuevo.FechaActualizacion = DateTime.Now;

                        await repoInventario.UpdateAsync(inventarioNuevo);
                    }

                    await repoMovimiento.CreateAsync(new MovimientoInventario
                    {
                        TipoMovimiento = "Entrada por cambio de almacén",
                        Cantidad = detalle.Cantidad,
                        Fecha = DateTime.Now,
                        Observacion = $"Compra #{compra.Id}",
                        InventarioId = inventarioNuevo.Id
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

            var detallesAgrupados = compra.DetalleCompras
                .GroupBy(d => d.ProductoId)
                .Select(g => new
                {
                    ProductoId = g.Key,
                    Cantidad = g.Sum(d => d.CantidadRecibida)
                })
                .ToList();

            var inventarios = await repoInventario.GetAllAsync();

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

                inventario.Stock -= detalle.Cantidad;
                inventario.FechaActualizacion = DateTime.Now;

                await repoInventario.UpdateAsync(inventario);

                await repoMovimiento.CreateAsync(new MovimientoInventario
                {
                    TipoMovimiento = "Salida por eliminación de compra",
                    Cantidad = detalle.Cantidad,
                    Fecha = DateTime.Now,
                    Observacion = $"Eliminación de compra #{compra.Id}",
                    InventarioId = inventario.Id
                });
            }

            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "No fue posible eliminar la compra");
        }
    }
}