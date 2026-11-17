using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IDevolucionesRepository
    {
       Task<IReadOnlyList<Devoluciones>> GetAllAsync();
        Task<Devoluciones?> GetByIdAsync(int id);
        Task<Devoluciones?> CreateAsync(Devoluciones devolucion);
        Task<bool> UpdateAsync(Devoluciones devolucion);
        Task<bool> DeleteAsync(int id);
    }
}
