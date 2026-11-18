using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class MovimientoInventarioService(
        IMovimientoInventarioRepository repo,
        IInventarioRepository repoInventario) : IMovimientoInventarioService
    {
        public async Task<IReadOnlyList<MovimientoInventarioTableDto>> GetAllAsync()
        {
            var movimientos = await repo.GetAllAsyc();

            return movimientos.Select(m => new MovimientoInventarioTableDto
            {
                Id = m.Id,
                TipoMovimiento = m.TipoMovimiento,
                Cantidad = m.Cantidad,
                Fecha = m.Fecha,
                Observacion = m.Observacion,
                InventarioId = m.InventarioId
            }).ToList();
        }

        public async Task<MovimientoInventarioDto?> GetByIdAsync(int id)
        {
            var movimiento = await repo.GetByIdAsync(id);

            if (movimiento == null)
                return null;

            return new MovimientoInventarioDto
            {
                Id = movimiento.Id,
                TipoMovimiento = movimiento.TipoMovimiento,
                Cantidad = movimiento.Cantidad,
                Fecha = movimiento.Fecha,
                Observacion = movimiento.Observacion,
                InventarioId = movimiento.InventarioId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateMovimientoInventarioDto dto)
        {
            string tipo = dto.TipoMovimiento.Trim();

            if (!EsTipoValido(tipo))
                return (false, "El tipo de movimiento debe ser Entrada o Salida.");

            if (dto.Cantidad <= 0)
                return (false, "La cantidad debe ser mayor que cero.");

            var inventario = await repoInventario.GetByIdAsync(dto.InventarioId);

            if (inventario == null)
                return (false, "El inventario seleccionado no existe.");

            int stockAnterior = inventario.Stock;
            int? stockNuevo = CalcularStock(
                stockAnterior, tipo, dto.Cantidad);

            if (stockNuevo == null)
                return (false, "No hay existencias suficientes para realizar esta salida.");

            inventario.Stock = stockNuevo.Value;
            inventario.FechaActualizacion = DateTime.Now;

            inventario.DetalleInventarios.Add(new DetalleInventario
            {
                SucursalId = inventario.Almacen.SucursalId,
                StockAnterior = stockAnterior,
                StockNuevo = inventario.Stock,
                FechaRegistro = DateTime.Now,
                Observacion = dto.Observacion?.Trim()
                    ?? $"Movimiento de {tipo}"
            });

            await repoInventario.UpdateAsync(inventario);

            var movimiento = new MovimientoInventario
            {
                TipoMovimiento = tipo,
                Cantidad = dto.Cantidad,
                Observacion = dto.Observacion?.Trim(),
                InventarioId = dto.InventarioId
            };

            await repo.CreateAsync(movimiento);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateMovimientoInventarioDto dto)
        {
            var movimiento = await repo.GetByIdAsync(id);

            if (movimiento == null)
                return (false, "El movimiento de inventario no existe.");

            string tipoNuevo = dto.TipoMovimiento.Trim();

            if (!EsTipoValido(tipoNuevo))
                return (false, "El tipo de movimiento debe ser Entrada o Salida.");

            if (dto.Cantidad <= 0)
                return (false, "La cantidad debe ser mayor que cero.");

            var inventarioAnterior = await repoInventario.GetByIdAsync(
                movimiento.InventarioId);

            if (inventarioAnterior == null)
                return (false, "No se encontró el inventario del movimiento original.");

            var inventarioNuevo = movimiento.InventarioId == dto.InventarioId
                ? inventarioAnterior
                : await repoInventario.GetByIdAsync(dto.InventarioId);

            if (inventarioNuevo == null)
                return (false, "El inventario seleccionado no existe.");

            int stockAnterior = inventarioAnterior.Stock;

            int? stockRevertido = CalcularStock(
                stockAnterior,
                movimiento.TipoMovimiento,
                movimiento.Cantidad,
                true);

            if (stockRevertido == null)
            {
                return (false,
                    "No se puede modificar el movimiento porque las existencias actuales no permiten revertirlo.");
            }

            int? stockFinal = CalcularStock(
                stockRevertido.Value,
                tipoNuevo,
                dto.Cantidad);

            if (stockFinal == null)
                return (false, "No hay existencias suficientes para realizar el nuevo movimiento.");

            if (inventarioAnterior.Id == inventarioNuevo.Id)
            {
                inventarioAnterior.Stock = stockFinal.Value;
                inventarioAnterior.FechaActualizacion = DateTime.Now;

                inventarioAnterior.DetalleInventarios.Add(new DetalleInventario
                {
                    SucursalId = inventarioAnterior.Almacen.SucursalId,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockFinal.Value,
                    FechaRegistro = DateTime.Now,
                    Observacion = "Actualización de movimiento de inventario"
                });

                await repoInventario.UpdateAsync(inventarioAnterior);
            }
            else
            {
                int stockNuevoInventarioAnterior = stockRevertido.Value;
                int stockActualNuevoInventario = inventarioNuevo.Stock;

                inventarioAnterior.Stock = stockNuevoInventarioAnterior;
                inventarioAnterior.FechaActualizacion = DateTime.Now;

                inventarioAnterior.DetalleInventarios.Add(new DetalleInventario
                {
                    SucursalId = inventarioAnterior.Almacen.SucursalId,
                    StockAnterior = stockAnterior,
                    StockNuevo = stockNuevoInventarioAnterior,
                    FechaRegistro = DateTime.Now,
                    Observacion = "Reversión por cambio de inventario"
                });

                inventarioNuevo.Stock = stockFinal.Value;
                inventarioNuevo.FechaActualizacion = DateTime.Now;

                inventarioNuevo.DetalleInventarios.Add(new DetalleInventario
                {
                    SucursalId = inventarioNuevo.Almacen.SucursalId,
                    StockAnterior = stockActualNuevoInventario,
                    StockNuevo = stockFinal.Value,
                    FechaRegistro = DateTime.Now,
                    Observacion = "Aplicación de movimiento en nuevo inventario"
                });

                await repoInventario.UpdateAsync(inventarioAnterior);
                await repoInventario.UpdateAsync(inventarioNuevo);
            }

            movimiento.TipoMovimiento = tipoNuevo;
            movimiento.Cantidad = dto.Cantidad;
            movimiento.Observacion = dto.Observacion?.Trim();
            movimiento.InventarioId = dto.InventarioId;

            await repo.UpdateAsync(movimiento);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var movimiento = await repo.GetByIdAsync(id);

            if (movimiento == null)
                return (false, "El movimiento de inventario no existe.");

            var inventario = await repoInventario.GetByIdAsync(
                movimiento.InventarioId);

            if (inventario == null)
                return (false, "No se encontró el inventario relacionado con el movimiento.");

            int stockAnterior = inventario.Stock;

            int? stockNuevo = CalcularStock(
                stockAnterior,
                movimiento.TipoMovimiento,
                movimiento.Cantidad,
                true);

            if (stockNuevo == null)
            {
                return (false,
                    "No se puede eliminar el movimiento porque las existencias actuales no permiten revertirlo.");
            }

            inventario.Stock = stockNuevo.Value;
            inventario.FechaActualizacion = DateTime.Now;

            inventario.DetalleInventarios.Add(new DetalleInventario
            {
                SucursalId = inventario.Almacen.SucursalId,
                StockAnterior = stockAnterior,
                StockNuevo = stockNuevo.Value,
                FechaRegistro = DateTime.Now,
                Observacion = "Reversión por eliminación de movimiento"
            });

            await repoInventario.UpdateAsync(inventario);
            await repo.DeleteAsync(id);

            return (true, null);
        }

        private static bool EsTipoValido(string tipo)
        {
            return tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase)
                || tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase);
        }

        private static int? CalcularStock(
            int stockActual,
            string tipo,
            int cantidad,
            bool revertir = false)
        {
            int variacion;

            if (tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                variacion = cantidad;
            else if (tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                variacion = -cantidad;
            else
                return null;

            if (revertir)
                variacion *= -1;

            int resultado = stockActual + variacion;

            return resultado < 0 ? null : resultado;
        }
    }
}
