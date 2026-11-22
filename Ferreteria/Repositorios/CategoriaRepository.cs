using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class CategoriaRepository(AppDbContext context) : ICategoriaRepository
    {
        public async Task<Categoria?> CreateAsync(Categoria categoria)
        {
            var entry = await context.Categorias.AddAsync(categoria);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Categoria>> GetAllAsync()
        {
            return await context.Categorias
                .AsNoTracking()
                .Include(c => c.Productos)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            return await context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateAsync(Categoria categoria)
        {
            context.Categorias.Update(categoria);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> HasProductosAsync(int categoriaId)
        {
            return await context.Productos
                .AnyAsync(p => p.CategoriaId == categoriaId);
        }


    }
}
