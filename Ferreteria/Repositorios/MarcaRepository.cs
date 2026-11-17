using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class MarcaRepository(AppDbContext context) : IMarcaRepository
    {
        public async Task<Marca?> CreateAsync(Marca marca)
        {
            var entry = await context.Marcas.AddAsync(marca);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var marca = await context.Marcas.FindAsync(id);

            if (marca == null) return false;

            context.Marcas.Remove(marca);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Marca>> GetAllAsync()
        {
            return await context.Marcas
                .AsNoTracking()
                .Include(m => m.Productos)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Marca?> GetByIdAsync(int id)
        {
            return await context.Marcas
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> UpdateAsync(Marca marca)
        {
            context.Marcas.Update(marca);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
