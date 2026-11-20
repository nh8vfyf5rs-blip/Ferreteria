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
        public async Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            return await repo.GetPagedAsync(request);
        }

        public async Task<IReadOnlyList<VentaTableDto>> GetAllAsync()
        {
            var ventas = await repo.GetAllAsync();

            return ventas.Select(v => new VentaTableDto
            {
                Id = v.Id,
                FechaVenta = v.FechaVenta,
                Total = v.Total,
                ClienteId = v.ClienteId,
                ClienteNombre = v.Cliente?.Name ?? string.Empty,
                EmpleadoNombre = v.Empleado != null
                    ? $"{v.Empleado.Nombres} {v.Empleado.Apellidos}".Trim()
                    : string.Empty,
                AlmacenNombre = v.Almacen?.Name ?? string.Empty,
                SucursalNombre = v.Almacen?.Sucursal?.Name ?? string.Empty,
                CantidadProductos = v.DetalleVentas?.Count ?? 0
            }).ToList();
        }

        public async Task<SaveVentaDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var venta = await repo.GetByIdAsync(id);

            if (venta == null)
                return null;

            return new SaveVentaDto
            {
                Id = venta.Id,
                FechaVenta = venta.FechaVenta,
                Total = venta.Total,
                ClienteId = venta.ClienteId,
                EmpleadoId = venta.EmpleadoId,
                AlmacenId = venta.AlmacenId,
                DetalleVentaDtos = venta.DetalleVentas.Select(d =>
                    new DetalleVentaDto
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            SaveVentaDto dto)
        {
            if (dto == null)
                return (false, "Los datos de la venta son obligatorios.");

            var error = await ValidateReferencesAsync(dto);

            if (error != null)
                return (false, error);

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = BuildStockChanges(null, dto);

            error = ValidateStockChanges(cambios, inventarios);

            if (error != null)
                return (false, error);

            var venta = new Venta
            {
                FechaVenta = dto.FechaVenta,
                ClienteId = dto.ClienteId,
                EmpleadoId = dto.EmpleadoId,
                AlmacenId = dto.AlmacenId,
                DetalleVentas = dto.DetalleVentaDtos.Select(d =>
                    new DetalleVenta
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    }).ToList()
            };

            venta.Total = venta.DetalleVentas.Sum(d => d.Subtotal);

            var creada = await repo.CreateAsync(venta);

            if (creada == null)
                return (false, "No fue posible registrar la venta.");

            error = await ApplyStockChangesAsync(
                cambios, inventarios, creada.Id);

            if (error != null)
                return (false, error);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            SaveVentaDto dto)
        {
            if (dto == null)
                return (false, "Los datos de la venta son obligatorios.");

            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "Debe indicar un identificador de venta válido.");

            var venta = await repo.GetByIdAsync(dto.Id.Value);

            if (venta == null)
                return (false, "La venta no existe.");

            var error = await ValidateReferencesAsync(dto);

            if (error != null)
                return (false, error);

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = BuildStockChanges(venta, dto);

            error = ValidateStockChanges(cambios, inventarios);

            if (error != null)
                return (false, error);

            venta.FechaVenta = dto.FechaVenta;
            venta.ClienteId = dto.ClienteId;
            venta.EmpleadoId = dto.EmpleadoId;
            venta.AlmacenId = dto.AlmacenId;

            venta.DetalleVentas.Clear();

            foreach (var detalle in dto.DetalleVentaDtos)
            {
                venta.DetalleVentas.Add(new DetalleVenta
                {
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario,
                    Subtotal = detalle.Cantidad * detalle.PrecioUnitario
                });
            }

            venta.Total = venta.DetalleVentas.Sum(d => d.Subtotal);

            var actualizado = await repo.UpdateAsync(venta);

            if (!actualizado)
                return (false, "No fue posible actualizar la venta.");

            error = await ApplyStockChangesAsync(
                cambios, inventarios, venta.Id);

            if (error != null)
                return (false, error);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (id <= 0)
                return (false, "El identificador de la venta no es válido.");

            var venta = await repo.GetByIdAsync(id);

            if (venta == null)
                return (false, "La venta no existe.");

            if (venta.DetallePagos.Any())
                return (false, "No se puede eliminar una venta que tiene pagos registrados.");

            var inventarios = await repoInventario.GetAllAsync();
            var cambios = BuildStockChanges(venta, null);

            var error = ValidateStockChanges(cambios, inventarios);

            if (error != null)
                return (false, error);

            var eliminada = await repo.DeleteAsync(id);

            if (!eliminada)
                return (false, "No fue posible eliminar la venta.");

            error = await ApplyStockChangesAsync(
                cambios, inventarios, id);

            if (error != null)
                return (false, error);

            return (true, null);
        }

        private async Task<string?> ValidateReferencesAsync(SaveVentaDto dto)
        {
            if (await repoCliente.GetByIdAsync(dto.ClienteId) == null)
                return "El cliente seleccionado no existe.";

            if (await repoEmpleado.GetByIdAsync(dto.EmpleadoId) == null)
                return "El empleado seleccionado no existe.";

            if (await repoAlmacen.GetByIdAsync(dto.AlmacenId) == null)
                return "El almacén seleccionado no existe.";

            if (dto.DetalleVentaDtos == null ||
                dto.DetalleVentaDtos.Count == 0)
                return "Debe agregar al menos un producto a la venta.";

            foreach (var detalle in dto.DetalleVentaDtos)
            {
                if (detalle.Cantidad <= 0)
                    return "La cantidad de cada producto debe ser mayor que cero.";

                if (detalle.PrecioUnitario <= 0)
                    return "El precio unitario debe ser mayor que cero.";

                if (await repoProducto.GetByIdAsync(detalle.ProductoId) == null)
                    return $"El producto con identificador {detalle.ProductoId} no existe.";
            }

            return null;
        }

        private static Dictionary<(int AlmacenId, int ProductoId), int>
            BuildStockChanges(Venta? ventaActual, SaveVentaDto? dto)
        {
            var cambios = new Dictionary<(int, int), int>();

            void AddChange(int almacenId, int productoId, int cantidad)
            {
                var key = (almacenId, productoId);

                if (!cambios.ContainsKey(key))
                    cambios[key] = 0;

                cambios[key] += cantidad;
            }

            if (ventaActual != null)
            {
                foreach (var detalle in ventaActual.DetalleVentas)
                {
                    AddChange(
                        ventaActual.AlmacenId,
                        detalle.ProductoId,
                        detalle.Cantidad);
                }
            }

            if (dto != null)
            {
                foreach (var detalle in dto.DetalleVentaDtos)
                {
                    AddChange(
                        dto.AlmacenId,
                        detalle.ProductoId,
                        -detalle.Cantidad);
                }
            }

            return cambios;
        }

        private static string? ValidateStockChanges(
            Dictionary<(int AlmacenId, int ProductoId), int> cambios,
            IReadOnlyList<Inventario> inventarios)
        {
            foreach (var cambio in cambios)
            {
                var inventario = inventarios.FirstOrDefault(i =>
                    i.AlmacenId == cambio.Key.AlmacenId &&
                    i.ProductoId == cambio.Key.ProductoId);

                if (inventario == null)
                {
                    return $"No existe inventario para el producto " +
                           $"{cambio.Key.ProductoId} en el almacén " +
                           $"{cambio.Key.AlmacenId}.";
                }

                if (inventario.Stock + cambio.Value < 0)
                {
                    return $"Stock insuficiente para el producto " +
                           $"{cambio.Key.ProductoId}. Disponible: " +
                           $"{inventario.Stock}.";
                }
            }

            return null;
        }

        private async Task<string?> ApplyStockChangesAsync(
            Dictionary<(int AlmacenId, int ProductoId), int> cambios,
            IReadOnlyList<Inventario> inventarios,
            int ventaId)
        {
            foreach (var cambio in cambios)
            {
                if (cambio.Value == 0)
                    continue;

                var inventario = inventarios.First(i =>
                    i.AlmacenId == cambio.Key.AlmacenId &&
                    i.ProductoId == cambio.Key.ProductoId);

                var almacen = await repoAlmacen.GetByIdAsync(cambio.Key.AlmacenId);
                int sucursalId = almacen?.SucursalId ?? inventario.Almacen?.SucursalId ?? 0;

                int stockAnterior = inventario.Stock;
                int stockNuevo = stockAnterior + cambio.Value;

                inventario.Stock = stockNuevo;
                inventario.FechaActualizacion = DateTime.Now;

                var actualizado = await repoInventario.UpdateAsync(inventario);

                if (!actualizado)
                    return $"No fue posible actualizar el inventario del producto {inventario.ProductoId}.";

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

                var registrado = await repoDetalle.CreateAsync(detalle);

                if (registrado == null)
                    return $"No fue posible registrar el detalle de inventario del producto {inventario.ProductoId}.";
            }

            return null;
        }
    }
}