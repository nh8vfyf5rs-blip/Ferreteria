using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Devoluciones>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task AddAsync(Producto Producto);
        Task Update(Producto Producto);
        Task Delete(Producto Producto);
        Task SaveAsync();
    }
}
