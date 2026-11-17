using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IClienteRepository
    {
        Task<IReadOnlyList<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> CreateAsync(Cliente cliente);
        Task<bool> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);
        Task<bool> CedulaExistsAsync(string cedula, int? excludeId = null);
        // Comprueba si el cliente tiene ventas asociadas.
        Task<bool> HasVentasAsync(int clienteId);

    }
}
