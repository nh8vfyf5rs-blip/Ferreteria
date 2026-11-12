using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class AlmacenRepository : IAlmacenRepository
    {
        public readonly AppDbContext _context;
        public AlmacenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Almacen Almacen)
        {
            await _context.Almacenes.AddAsync(Almacen);
        }

        public async Task Delete(Almacen Almacen)
        {
            _context.Almacenes.Remove(Almacen);
        }

        public async Task<IEnumerable<Almacen>> GetAllAsync()
        {
            return await _context.Almacenes
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Almacen?> GetByIdAsync(int id)
        {
            return await _context.Almacenes.FindAsync(id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Almacen Almacen)
        {
            _context.Almacenes.Update(Almacen);
        }
    }
}
