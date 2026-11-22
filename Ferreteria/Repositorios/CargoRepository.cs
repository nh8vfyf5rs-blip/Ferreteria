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
                    .AsNoTracking()
                    .Include(c => c.Empleados)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }

            public async Task<Cargo?> GetByIdAsync(int id)
            {
                return await context.Cargos
                    .Include(c => c.Empleados)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }

            public async Task<bool> UpdateAsync(Cargo cargo)
            {
                context.Cargos.Update(cargo);
                await context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> NombreExisteAsync(string name, int? excludeId = null)
            {
                var normalized = name.Trim().ToLower();

                return await context.Cargos
                    .AsNoTracking()
                    .AnyAsync(c =>
                        c.Name.ToLower() == normalized &&
                        (!excludeId.HasValue || c.Id != excludeId.Value));
            }

            public async Task<bool> HasAnyEmpleadoAsync(int cargoId)
            {
                return await context.Empleados
                    .AsNoTracking()
                    .AnyAsync(e => e.CargoId == cargoId);
            }

            public async Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request)
            {
                var query = context.Cargos
                    .AsNoTracking()
                    .Select(c => new CargoTableDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        TotalEmpleados = c.Empleados.Count
                    });

                var totalRecords = await query.CountAsync();

                var searchValue = request.Search?.Value?.Trim();
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    var searchLower = searchValue.ToLower();
                    query = query.Where(c => c.Name.ToLower().Contains(searchLower));
                }

                var filteredRecords = await query.CountAsync();

                query = query.OrderBy(c => c.Name);

                var data = await query
                    .Skip(Math.Max(0, request.Start))
                    .Take(request.Length > 0 ? request.Length : 10)
                    .ToListAsync();

                return new DataTableResponseDto<CargoTableDto>
                {
                    Draw = request.Draw,
                    RecordsTotal = totalRecords,
                    RecordsFiltered = filteredRecords,
                    Data = data
                };
            }
        }
    }

