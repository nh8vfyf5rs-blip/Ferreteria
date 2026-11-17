using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
    }
}
