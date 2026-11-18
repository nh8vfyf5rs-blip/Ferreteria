using Ferreteria.Data;
using Ferreteria.DTOS;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class CargoRepository(AppDbContext context) : ICargoRepository
    {
        public async Task<Cargo?> CreateAsync(Cargo cargo)
        {
            var entry = await context.Cargos.AddAsync(cargo);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cargo = await context.Cargos.FindAsync(id);

            if (cargo == null)
                return false;

            context.Cargos.Remove(cargo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Cargo>> GetAllAsync()
        {
            return await context.Cargos
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Cargo?> GetByIdAsync(int id)
        {
            return await context.Cargos.FindAsync(id);
        }

        public async Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(
            DataTablesRequestDto request)
        {
            var query = context.Cargos.AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search));
            }

            var recordsFiltered = await query.CountAsync();

            var order = request.Order.FirstOrDefault();
            var orderColumn = order?.column ?? 0;
            var orderDir = order?.Dir ?? "asc";

            var columnName = request.Columns
                .ElementAtOrDefault(orderColumn)?.Data ?? "name";

            query = columnName.ToLowerInvariant() switch
            {
                "id" => orderDir == "desc"
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),

                "totalempleados" => orderDir == "desc"
                    ? query.OrderByDescending(c => c.Empleados.Count)
                    : query.OrderBy(c => c.Empleados.Count),

                _ => orderDir == "desc"
                    ? query.OrderByDescending(c => c.Name)
                    : query.OrderBy(c => c.Name)
            };

            var data = await query
                .Skip(request.Start)
                .Take(request.Length > 0 ? request.Length : 10)
                .Select(c => new CargoTableDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalEmpleados = c.Empleados.Count
                })
                .ToListAsync();

            return new DataTableResponseDto<CargoTableDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> NombreExisteAsync(
            string nombre, int? excludeId = null)
        {
            return await context.Cargos.AnyAsync(c =>
                c.Name == nombre &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> HasAnyEmpleadoAsync(int id)
        {
            return await context.Empleados
                .AnyAsync(e => e.CargoId == id);
        }

        public async Task<bool> UpdateAsync(Cargo cargo)
        {
            context.Cargos.Update(cargo);
            await context.SaveChangesAsync();

            return true;
        }
    }
}