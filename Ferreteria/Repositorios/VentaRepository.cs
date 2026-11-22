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
                    .ThenInclude(a => a.Sucursal)
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
                    .ThenInclude(a => a.Sucursal)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.DetallePagos)
                    .ThenInclude(p => p.MetodoPago)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public  async Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            var query = context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Almacen)
                    .ThenInclude(a => a.Sucursal)
                .Include(v => v.DetalleVentas)
                .AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(v =>
                    (v.Cliente != null && (v.Cliente.Name ?? "").ToLower().Contains(term)) ||
                    (v.Empleado != null && (v.Empleado.Nombres ?? "").ToLower().Contains(term)) ||
                    (v.Almacen != null && (v.Almacen.Name ?? "").ToLower().Contains(term)) ||
                    (v.Almacen != null && v.Almacen.Sucursal != null && (v.Almacen.Sucursal.Name ?? "").ToLower().Contains(term)));
            }

            var recordsFiltered = await query.CountAsync();

            var start = request.Start >= 0 ? request.Start : 0;
            var length = request.Length > 0 ? request.Length : 10;

            var data = await query
                .OrderByDescending(v => v.FechaVenta)
                .Skip(start)
                .Take(length)
                .Select(v => new VentaTableDto
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