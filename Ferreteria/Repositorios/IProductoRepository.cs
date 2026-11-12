using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task AddAsync(Producto Producto);
        Task Update(Producto Producto);
        Task Delete(Producto Producto);
        Task SaveAsync();
    
}
}
