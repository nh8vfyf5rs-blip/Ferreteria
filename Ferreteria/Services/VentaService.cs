using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class VentaService(
        IVentaRepository repo,
        IClienteRepository repoCliente,
        IEmpleadoRepository repoEmpleado,
        IAlmacenRepository repoAlmacen,
        IProductoRepository repoProducto,
        IInventarioRepository repoInventario,
        IDetalleInventarioRepository repoDetalle) : IVentaService
    {
        public async Task<IReadOnlyList<VentaTableDto>> GetAllAsync()
        {
            var ventas = await repo.GetAllAsync();

            return ventas.Select(v => new VentaTableDto
            {
                Id = v.Id,
                FechaVenta = v.FechaVenta,
                Total = v.Total,
                ClienteId = v.ClienteId,
                ClienteNombre = v.Cliente != null ? (v.Cliente.Name ?? string.Empty) : string.Empty,
                EmpleadoNombre = v.Empleado != null ? (v.Empleado.Nombres ?? string.Empty) : string.Empty,
                AlmacenNombre = v.Almacen != null ? (v.Almacen.Name ?? string.Empty) : string.Empty,
                SucursalNombre = v.Almacen != null && v.Almacen.Sucursal != null
                    ? (v.Almacen.Sucursal.Name ?? string.Empty)
                    : string.Empty,
                CantidadProductos = v.DetalleVentas != null ? v.DetalleVentas.Sum(dv => dv.Cantidad) : 0
            }).ToList();
        }

        public async Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            return await repo.GetPagedAsync(request);
        }

        public async Task<SaveVentaDto?> GetByIdAsync(int id)
        {
            var v = await repo.GetByIdAsync(id);
            if (v == null) return null;

            var primerPago = v.DetallePagos?.FirstOrDefault();

            return new SaveVentaDto
            {
                Id = v.Id,
                FechaVenta = v.FechaVenta,
                Total = v.Total,
                ClienteId = v.ClienteId,
                EmpleadoId = v.EmpleadoId,
                AlmacenId = v.AlmacenId,
                MetodoPagoId = primerPago?.MetodoPagoId ?? 0,
                NumeroComprobante = primerPago?.NumeroComprobante,
                DetalleVentaDtos = v.DetalleVentas.Select(d => new DetalleVentaDto
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveVentaDto dto)
        {
            if (dto == null)
                return (false, "Los datos de la venta son obligatorios.");

            var (refError, sucursalPorAlmacen, nombresProductos, nombresAlmacenes) = await ValidateAndGetContextAsync(dto);
            if (refError != null)
                return (false, refError);

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = BuildStockChanges(null, dto);

            var stockError = ValidateStockChanges(cambios, inventarios, nombresProductos, nombresAlmacenes);
            if (stockError != null)
                return (false, stockError);

            var fechaVenta = dto.FechaVenta == default ? DateTime.Now : dto.FechaVenta;
            var comprobante = string.IsNullOrWhiteSpace(dto.NumeroComprobante)
                ? null
                : dto.NumeroComprobante.Trim();

            var venta = new Venta
            {
                FechaVenta = fechaVenta,
                ClienteId = dto.ClienteId,
                EmpleadoId = dto.EmpleadoId,
                AlmacenId = dto.AlmacenId,
                DetalleVentas = dto.DetalleVentaDtos.Select(d => new DetalleVenta
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Cantidad * d.PrecioUnitario
                }).ToList()
            };

            venta.Total = venta.DetalleVentas.Sum(d => d.Subtotal);

            if (dto.MetodoPagoId > 0)
            {
                venta.DetallePagos.Add(new DetallePago
                {
                    Venta = venta,
                    MetodoPagoId = dto.MetodoPagoId,
                    Monto = venta.Total,
                    FechaPago = fechaVenta,
                    NumeroComprobante = comprobante
                });
            }

            var creada = await repo.CreateAsync(venta);
            if (creada == null)
                return (false, "No fue posible registrar la venta.");

            var applyError = await ApplyStockChangesAsync(cambios, inventarios, sucursalPorAlmacen, nombresProductos, creada.Id);
            if (applyError != null)
                return (false, applyError);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveVentaDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "Id de venta no válido.");

            var venta = await repo.GetByIdAsync(dto.Id.Value);
            if (venta == null)
                return (false, "Venta no encontrada.");

            var (refError, sucursalPorAlmacen, nombresProductos, nombresAlmacenes) = await ValidateAndGetContextAsync(dto, venta);
            if (refError != null)
                return (false, refError);

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = BuildStockChanges(venta, dto);

            var stockError = ValidateStockChanges(cambios, inventarios, nombresProductos, nombresAlmacenes);
            if (stockError != null)
                return (false, stockError);

            venta.FechaVenta = dto.FechaVenta == default ? venta.FechaVenta : dto.FechaVenta;
            venta.ClienteId = dto.ClienteId;
            venta.EmpleadoId = dto.EmpleadoId;
            venta.AlmacenId = dto.AlmacenId;

            var comprobante = string.IsNullOrWhiteSpace(dto.NumeroComprobante)
                ? null
                : dto.NumeroComprobante.Trim();

            var nuevosDetalles = dto.DetalleVentaDtos.Select(d => new DetalleVenta
            {
                VentaId = venta.Id,
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Cantidad * d.PrecioUnitario
            }).ToList();

            venta.DetalleVentas.Clear();
            foreach (var det in nuevosDetalles)
            {
                venta.DetalleVentas.Add(det);
            }

            venta.Total = venta.DetalleVentas.Sum(d => d.Subtotal);

            if (dto.MetodoPagoId > 0)
            {
                var primerPago = venta.DetallePagos?.FirstOrDefault();
                if (primerPago != null)
                {
                    primerPago.MetodoPagoId = dto.MetodoPagoId;
                    primerPago.Monto = venta.Total;
                    primerPago.FechaPago = venta.FechaVenta;
                    primerPago.NumeroComprobante = comprobante;
                }
                else
                {
                    venta.DetallePagos!.Add(new DetallePago
                    {
                        Venta = venta,
                        MetodoPagoId = dto.MetodoPagoId,
                        Monto = venta.Total,
                        FechaPago = venta.FechaVenta,
                        NumeroComprobante = comprobante
                    });
                }
            }

            var actualizada = await repo.UpdateAsync(venta);
            if (!actualizada)
                return (false, "No fue posible actualizar la venta.");

            var applyError = await ApplyStockChangesAsync(cambios, inventarios, sucursalPorAlmacen, nombresProductos, venta.Id);
            if (applyError != null)
                return (false, applyError);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var venta = await repo.GetByIdAsync(id);
            if (venta == null)
                return (false, "Venta no encontrada.");

            var almacen = await repoAlmacen.GetByIdAsync(venta.AlmacenId);
            int sucursalId = almacen?.SucursalId ?? 0;
            var sucursalPorAlmacen = new Dictionary<int, int> { [venta.AlmacenId] = sucursalId };
            var nombresAlmacenes = new Dictionary<int, string>
            {
                [venta.AlmacenId] = string.IsNullOrWhiteSpace(almacen?.Name) ? $"#{venta.AlmacenId}" : almacen.Name!
            };
            var nombresProductos = new Dictionary<int, string>();

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = new Dictionary<(int AlmacenId, int ProductoId), int>();

            foreach (var det in venta.DetalleVentas)
            {
                var producto = await repoProducto.GetByIdAsync(det.ProductoId);
                nombresProductos[det.ProductoId] = string.IsNullOrWhiteSpace(producto?.Nombre)
                    ? $"ID {det.ProductoId}"
                    : $"{producto.Nombre} (ID {det.ProductoId})";

                var key = (venta.AlmacenId, det.ProductoId);
                cambios[key] = cambios.GetValueOrDefault(key) + det.Cantidad;
            }

            var stockError = ValidateStockChanges(cambios, inventarios, nombresProductos, nombresAlmacenes);
            if (stockError != null)
                return (false, stockError);

            var applyError = await ApplyStockChangesAsync(cambios, inventarios, sucursalPorAlmacen, nombresProductos, venta.Id);
            if (applyError != null)
                return (false, applyError);

            var eliminada = await repo.DeleteAsync(id);
            return eliminada
                ? (true, null)
                : (false, "No fue posible eliminar la venta.");
        }

        private async Task<(
            string? Error,
            Dictionary<int, int> SucursalPorAlmacen,
            Dictionary<int, string> NombresProductos,
            Dictionary<int, string> NombresAlmacenes
        )> ValidateAndGetContextAsync(SaveVentaDto dto, Venta? ventaAnterior = null)
        {
            var sucursales = new Dictionary<int, int>();
            var nombresProductos = new Dictionary<int, string>();
            var nombresAlmacenes = new Dictionary<int, string>();

            if (await repoCliente.GetByIdAsync(dto.ClienteId) == null)
                return ("El cliente seleccionado no existe.", sucursales, nombresProductos, nombresAlmacenes);

            if (await repoEmpleado.GetByIdAsync(dto.EmpleadoId) == null)
                return ("El empleado seleccionado no existe.", sucursales, nombresProductos, nombresAlmacenes);

            var almacen = await repoAlmacen.GetByIdAsync(dto.AlmacenId);
            if (almacen == null)
                return ("El almacén seleccionado no existe.", sucursales, nombresProductos, nombresAlmacenes);

            if (almacen.SucursalId <= 0)
                return ("El almacén seleccionado no tiene una sucursal válida asignada.", sucursales, nombresProductos, nombresAlmacenes);

            sucursales[dto.AlmacenId] = almacen.SucursalId;
            nombresAlmacenes[dto.AlmacenId] = string.IsNullOrWhiteSpace(almacen.Name)
                ? $"#{dto.AlmacenId}"
                : $"{almacen.Name} (#{dto.AlmacenId})";

            if (ventaAnterior != null && ventaAnterior.AlmacenId != dto.AlmacenId)
            {
                var almacenAnterior = await repoAlmacen.GetByIdAsync(ventaAnterior.AlmacenId);
                if (almacenAnterior != null)
                {
                    sucursales[ventaAnterior.AlmacenId] = almacenAnterior.SucursalId;
                    nombresAlmacenes[ventaAnterior.AlmacenId] = string.IsNullOrWhiteSpace(almacenAnterior.Name)
                        ? $"#{ventaAnterior.AlmacenId}"
                        : $"{almacenAnterior.Name} (#{ventaAnterior.AlmacenId})";
                }
            }

            if (dto.DetalleVentaDtos == null || dto.DetalleVentaDtos.Count == 0)
                return ("Debe agregar al menos un producto a la venta.", sucursales, nombresProductos, nombresAlmacenes);

            foreach (var d in dto.DetalleVentaDtos)
            {
                if (d.ProductoId <= 0)
                    return ("Debe seleccionar un producto válido.", sucursales, nombresProductos, nombresAlmacenes);

                if (d.Cantidad <= 0)
                    return ("La cantidad debe ser mayor a cero.", sucursales, nombresProductos, nombresAlmacenes);

                if (d.PrecioUnitario <= 0)
                    return ("El precio unitario debe ser mayor a cero.", sucursales, nombresProductos, nombresAlmacenes);

                var producto = await repoProducto.GetByIdAsync(d.ProductoId);
                if (producto == null)
                    return ($"El producto con ID {d.ProductoId} no existe.", sucursales, nombresProductos, nombresAlmacenes);

                nombresProductos[d.ProductoId] = string.IsNullOrWhiteSpace(producto.Nombre)
                    ? $"ID {d.ProductoId}"
                    : $"{producto.Nombre} (ID {d.ProductoId})";
            }

            if (ventaAnterior != null)
            {
                foreach (var detAnt in ventaAnterior.DetalleVentas)
                {
                    if (!nombresProductos.ContainsKey(detAnt.ProductoId))
                    {
                        var prodAnt = await repoProducto.GetByIdAsync(detAnt.ProductoId);
                        nombresProductos[detAnt.ProductoId] = string.IsNullOrWhiteSpace(prodAnt?.Nombre)
                            ? $"ID {detAnt.ProductoId}"
                            : $"{prodAnt.Nombre} (ID {detAnt.ProductoId})";
                    }
                }
            }

            return (null, sucursales, nombresProductos, nombresAlmacenes);
        }

        private static Dictionary<(int AlmacenId, int ProductoId), int> BuildStockChanges(
            Venta? ventaAnterior,
            SaveVentaDto dtoNuevo)
        {
            var cambios = new Dictionary<(int AlmacenId, int ProductoId), int>();

            if (ventaAnterior != null)
            {
                foreach (var det in ventaAnterior.DetalleVentas)
                {
                    var keyAnterior = (ventaAnterior.AlmacenId, det.ProductoId);
                    cambios[keyAnterior] = cambios.GetValueOrDefault(keyAnterior) + det.Cantidad;
                }
            }

            foreach (var det in dtoNuevo.DetalleVentaDtos)
            {
                var keyNueva = (dtoNuevo.AlmacenId, det.ProductoId);
                cambios[keyNueva] = cambios.GetValueOrDefault(keyNueva) - det.Cantidad;
            }

            return cambios;
        }

        private static string? ValidateStockChanges(
            Dictionary<(int AlmacenId, int ProductoId), int> cambios,
            IReadOnlyList<Inventario> inventarios,
            Dictionary<int, string> nombresProductos,
            Dictionary<int, string> nombresAlmacenes)
        {
            foreach (var cambio in cambios)
            {
                if (cambio.Value == 0)
                    continue;

                var inventario = inventarios.FirstOrDefault(i =>
                    i.AlmacenId == cambio.Key.AlmacenId &&
                    i.ProductoId == cambio.Key.ProductoId);

                string prodLabel = nombresProductos.TryGetValue(cambio.Key.ProductoId, out var pName)
                    ? pName
                    : $"ID {cambio.Key.ProductoId}";

                string almLabel = nombresAlmacenes.TryGetValue(cambio.Key.AlmacenId, out var aName)
                    ? aName
                    : $"#{cambio.Key.AlmacenId}";

                if (inventario == null)
                    return $"No existe inventario registrado para {prodLabel} en el almacén {almLabel}.";

                if (inventario.Stock + cambio.Value < 0)
                    return $"Stock insuficiente para {prodLabel} en el almacén {almLabel} (disponible: {inventario.Stock}, solicitado: {Math.Abs(cambio.Value)}).";
            }

            return null;
        }

        private async Task<string?> ApplyStockChangesAsync(
            Dictionary<(int AlmacenId, int ProductoId), int> cambios,
            IReadOnlyList<Inventario> inventarios,
            Dictionary<int, int> sucursalPorAlmacen,
            Dictionary<int, string> nombresProductos,
            int ventaId)
        {
            foreach (var cambio in cambios)
            {
                if (cambio.Value == 0)
                    continue;

                var inventario = inventarios.First(i =>
                    i.AlmacenId == cambio.Key.AlmacenId &&
                    i.ProductoId == cambio.Key.ProductoId);

                int sucursalId = sucursalPorAlmacen.GetValueOrDefault(cambio.Key.AlmacenId);
                if (sucursalId <= 0)
                    sucursalId = inventario.Almacen?.SucursalId ?? inventario.SucursalId;

                if (sucursalId <= 0)
                    return $"El almacén #{cambio.Key.AlmacenId} no tiene una sucursal válida asignada.";

                if (inventario.SucursalId <= 0)
                    inventario.SucursalId = sucursalId;

                int stockAnterior = inventario.Stock;
                int stockNuevo = stockAnterior + cambio.Value;

                inventario.Stock = stockNuevo;
                inventario.FechaActualizacion = DateTime.Now;

                var actualizado = await repoInventario.UpdateAsync(inventario);
                if (!actualizado)
                {
                    string prodLabel = nombresProductos.TryGetValue(inventario.ProductoId, out var pName)
                        ? pName
                        : $"ID {inventario.ProductoId}";
                    return $"No fue posible actualizar el inventario de {prodLabel}.";
                }

                string accion = cambio.Value < 0
                    ? $"Salida por venta #{ventaId} (cantidad: {Math.Abs(cambio.Value)})."
                    : $"Ajuste por actualización o eliminación de venta #{ventaId} (cantidad: {cambio.Value}).";

                var detalle = new DetalleInventario
                {
                    InventarioId = inventario.Id,
                    SucursalId = sucursalId,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockNuevo,
                    FechaRegistro = DateTime.Now,
                    Observacion = accion
                };

                await repoDetalle.CreateAsync(detalle);
            }

            return null;
        }
    }
}