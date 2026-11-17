using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IMetodoPagoRepository
    {
        Task<IReadOnlyList<MetodoPago>> GetAllAsync();
        Task<MetodoPago?> GetByIdAsync(int id);
        Task<MetodoPago?> CreateAsync(MetodoPago metodoPago);
        Task<bool> UpdateAsync(MetodoPago metodoPago);
        Task<bool> DeleteAsync(int id);
    }
}
