using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IMarcaRepository
    {
        Task<IReadOnlyList<Marca>> GetAllAsync();

        Task<Marca?> GetByIdAsync(int id);

        Task<Marca?> CreateAsync(Marca marca);

        Task<bool> UpdateAsync(Marca marca);

        Task<bool> DeleteAsync(int id);
    }
}
