using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ICategoriaRepository
    {
        Task<IReadOnlyList<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria?> CreateAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasProductosAsync(int categoriaId);
    }
}
