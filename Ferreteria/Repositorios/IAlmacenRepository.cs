using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IAlmacenRepository
    {
        Task<IReadOnlyList<Almacen>> GetAllAsync();
        Task<Almacen?> GetByIdAsync(int id);
        Task<Almacen?> CreateAsync(Almacen almacen);
        Task<bool> UpdateAsync(Almacen almacen);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasInventariosAsync(int almacenId);
    }
}
