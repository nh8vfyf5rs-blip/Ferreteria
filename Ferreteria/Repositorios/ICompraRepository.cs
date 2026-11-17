using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ICompraRepository
    {
        Task<IReadOnlyList<Compra>> GetAllAsync();
        Task<Compra?> GetByIdAsync(int id);
        Task<Compra?> CreateAsync(Compra compra);
        Task<bool> UpdateAsync(Compra compra);
        Task<bool> DeleteAsync(int id);
    }
}
