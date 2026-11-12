using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IAlmacenRepository
    {
        Task<IEnumerable<Almacen>> GetAllAsync();
        Task<Almacen?> GetByIdAsync(int id);
        Task AddAsync(Almacen Almacen);
        Task Update(Almacen Almacen);
        Task Delete(Almacen Almacen);
        Task SaveAsync();
    }
}
