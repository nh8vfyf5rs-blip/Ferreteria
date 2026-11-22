using Ferreteria.Models.Segurudad;

namespace Ferreteria.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<Usuario>> GetAllAsync();
        Task<IReadOnlyList<Usuario>> GetAllUsuarioAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetUsuarioByIdWhitPermissionAsync(int userId);
        Task<Usuario?> GetUsuarioByUserName(string userName);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);
        Task<Usuario?> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
        void Delete(Usuario usuario);
        bool ValidatePassword(Usuario usuario, string password);
        Task SaveAsync();
        Task<bool> UpdateUsuarioRoleAsync(int usuarioId, int rolId);
        Task<bool> AssingPermissionToUsuarioAsync(int usuarioId, int permisoId, bool isGranted);
        Task<bool> RemovePermissionFromeUsuarioAsync(int usuarioId, int permisoId);
    }
}