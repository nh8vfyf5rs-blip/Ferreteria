using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IDevolucionesRepository
    {
        Task<IEnumerable<Devoluciones>> GetAllAsync();
        Task<Devoluciones?> GetByIdAsync(int id);
        Task AddAsync(Devoluciones devoluciones);
        Task Update(Devoluciones devoluciones);
        Task Delete(Devoluciones devoluciones);
        Task SaveAsync();
    }
}
