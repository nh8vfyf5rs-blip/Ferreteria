using Ferreteria.Data;
using Ferreteria.Models;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Repositorios
{
    public class ClienteRepository(AppDbContext context) : IClienteRepository
    {
        public async Task<Cliente?> CreateAsync(Cliente cliente)
        {
            var entry = await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await context.Clientes.FindAsync(id);

            if (cliente == null)
                return false;

            context.Clientes.Remove(cliente);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<Cliente>> GetAllAsync()
        {
            return await context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            context.Clientes.Update(cliente);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CedulaExistsAsync(
            string cedula,
            int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            var cedulaNormalizada = cedula.Trim();

            var query = context.Clientes
                .AsNoTracking()
                .Where(c => c.Cedula == cedulaNormalizada);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasVentasAsync(int clienteId)
        {
            return await context.Ventas
                .AsNoTracking()
                .AnyAsync(v => v.ClienteId == clienteId);
        }
    }
}

