using Ferreteria.Data;
using Ferreteria.DTOS;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class PermisoRepository(AppDbContext context) : IPermisoRepository
    {
        public async Task<Permiso> CreateAsync(string name, string? description)
        {
            var permission = new Permiso
            {
                Name = name.Trim().ToLowerInvariant(),
                Descripcion = description?.Trim()
            };

            var entry = await context.Permisos.AddAsync(permission);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<Permiso?> CreateAsync(Permiso permiso)
        {
            var entry = await context.Permisos.AddAsync(permiso);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var permission = await context.Permisos.FindAsync(id);
            if (permission == null)
                return false;

            context.Permisos.Remove(permission);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Permiso>> GetAllAsync()
        {
            return await context.Permisos
                .AsNoTracking()
                .Include(p => p.RolPermissions)
                    .ThenInclude(rp => rp.Rol)
                .Include(p => p.UsuarioPermisos)
                    .ThenInclude(up => up.Usuario)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Permiso?> GetByIdAsync(int id)
        {
            return await context.Permisos
                .Include(p => p.RolPermissions)
                    .ThenInclude(rp => rp.Rol)
                .Include(p => p.UsuarioPermisos)
                    .ThenInclude(up => up.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Permiso?> GetByNameAsync(string name)
        {
            var normalized = name.Trim();
            return await context.Permisos
                .FirstOrDefaultAsync(x => x.Name == normalized);
        }

        public async Task<DataTableResponseDto<PermisoDto>> GetPagedAsync(DataTablesRequestDto request)
        {
            var query = context.Permisos.AsNoTracking().AsQueryable();

            var recordsTotal = await query.CountAsync();

            var search = request.Search?.Value?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    (c.Descripcion != null && c.Descripcion.Contains(search)));
            }

            var recordsFiltered = await query.CountAsync();

            var orderColumn = request.Order?.FirstOrDefault()?.Column ?? 0;
            var orderDir = (request.Order?.FirstOrDefault()?.Dir ?? "asc").ToLowerInvariant();
            var columnName = request.Columns?.ElementAtOrDefault(orderColumn)?.Data ?? "name";

            var isDesc = orderDir is "desc" or "dsc";

            query = columnName.ToLowerInvariant() switch
            {
                "descripcion" or "description" => isDesc
                    ? query.OrderByDescending(c => c.Descripcion)
                    : query.OrderBy(c => c.Descripcion),
                "id" => isDesc
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),
                _ => isDesc
                    ? query.OrderByDescending(c => c.Name)
                    : query.OrderBy(c => c.Name)
            };

            var pageSize = request.Length > 0 ? request.Length : 10;
            var skip = request.Start > 0 ? request.Start : 0;

            var data = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(c => new PermisoDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Descripcion
                })
                .ToListAsync();

            return new DataTableResponseDto<PermisoDto>
            {
                Draw = request.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = data
            };
        }

        public async Task<bool> IsAssingedToUsuarioAsync(int permisoId)
        {
            var enUsuario = await context.UsuariosPermisos
                .AnyAsync(up => up.PermisoId == permisoId);

            if (enUsuario)
                return true;

            return await context.Permisos
                .Where(p => p.Id == permisoId)
                .SelectMany(p => p.RolPermissions)
                .AnyAsync();
        }

        public async Task<bool> UpdateAsync(Permiso permission)
        {
            context.Permisos.Update(permission);
            await context.SaveChangesAsync();

            return true;
        }
    }
}