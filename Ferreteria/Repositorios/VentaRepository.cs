using Ferreteria.Data;
using Ferreteria.DTOS;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class VentaRepository(AppDbContext context) : IVentaRepository
    {
        public async Task<Venta?> CreateAsync(Venta venta)
        {
            var entry = await context.Ventas.AddAsync(venta);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venta = await context.Ventas.FindAsync(id);

            if (venta == null)
                return false;

            context.Ventas.Remove(venta);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Venta>> GetAllAsync()
        {
            return await context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Almacen)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetallePagos)
                    .ThenInclude(p => p.MetodoPago)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();
        }

        public async Task<Venta?> GetByIdAsync(int id)
        {
            return await context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Almacen)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetallePagos)
                    .ThenInclude(p => p.MetodoPago)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(
            DataTablesRequestDto request)
        {
            var query = context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(v =>
                    v.Cliente.Name.Contains(search));
            }

            var recordsFiltered = await query.CountAsync();

            var orderColumn = request.Order.FirstOrDefault()?.column ?? 0;
            var orderDir = request.Order.FirstOrDefault()?.Dir ?? "asc";
            var columnName = request.Columns
                .ElementAtOrDefault(orderColumn)?.Data ?? "fechaVenta";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => orderDir == "desc"
                    ? query.OrderByDescending(v => v.Id)
                    : query.OrderBy(v => v.Id),

                "total" => orderDir == "desc"
                    ? query.OrderByDescending(v => v.Total)
                    : query.OrderBy(v => v.Total),

                "cliente" => orderDir == "desc"
                    ? query.OrderByDescending(v => v.Cliente.Name)
                    : query.OrderBy(v => v.Cliente.Name),

                "cantidadproductos" => orderDir == "desc"
                    ? query.OrderByDescending(v => v.DetalleVentas.Count())
                    : query.OrderBy(v => v.DetalleVentas.Count()),

                _ => orderDir == "desc"
                    ? query.OrderByDescending(v => v.FechaVenta)
                    : query.OrderBy(v => v.FechaVenta)
            };

            var data = await query
                .Skip(Math.Max(request.Start, 0))
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(v => new VentaTableDto
                {
                    Id = v.Id,
                    FechaVenta = v.FechaVenta,
                    Total = v.Total,
                    ClienteId = v.ClienteId,
                    Cliente = v.Cliente.Name,
                    CantidadProductos = v.DetalleVentas.Count()
                })
                .ToListAsync();

            return new DataTableResponseDto<VentaTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> UpdateAsync(Venta venta)
        {
            context.Ventas.Update(venta);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
