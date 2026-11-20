using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class EmpleadoRepository(AppDbContext context) : IEmpleadoRepository
    {
        public async Task<Empleado?> CreateAsync(Empleado empleado)
        {
            var entry = await context.Empleados.AddAsync(empleado);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
           var empleado = await context.Empleados.FindAsync(id);
            if (empleado == null) return false;

            context.Empleados.Remove(empleado);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Empleado>> GetAllAsync()
        {
            return await context.Empleados
                .AsNoTracking()
                .Include(e => e.Cargo)
                .Include(e => e.Sucursal)
                .OrderBy(e => e.Apellidos)
                .OrderBy(e => e.Nombres)
                .ToListAsync();
        }

        public async Task<Empleado?> GetByIdAsync(int id)
        {
            return await context.Empleados
                .Include(e => e.Cargo)
                .Include(e => e.Sucursal)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public  async Task<bool> HasComprasAsync(int empleadoId)
        {
            return await context.Compras.AnyAsync(c => c.EmpleadoId == empleadoId);
        }

        public async Task<bool> UpdateAsync(Empleado empleado)
        {
            context.Empleados.Update(empleado);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
