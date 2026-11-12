using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        void UpdateAsync(Cliente cliente);
        void Delete(Cliente cliente);
        Task SaveAsync();

    }
}
