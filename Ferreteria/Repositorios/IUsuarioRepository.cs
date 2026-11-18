using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IUsuarioRepository
    {
        // Operaciones CRUD existentes
        Task<IReadOnlyList<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);

        // Consultas para autenticación
        Task<Usuario?> GetUsuarioByUserName(string username);
        Task<Usuario?> GetUsuarioByEmail(string email);
        Task<Usuario?> GetUsuarioByIdWhitPermissionAsync(int id);
        Task<Usuario?> GetUsuarioByRefreshTokenAsync(string refreshToken);

        // Administración de usuarios y permisos
        Task<IReadOnlyList<Usuario>> GetAllUsuariosWhitRolesAsync();
        Task<bool> AssingPermissionToUsuarioAsync(
            int usuarioId, int permissionId, bool isGranted);
        Task<bool> RemovePermissionFromeUsuarioAsync(
            int usuarioId, int permissionId);
        Task<bool> UpdateUsuarioRoleAsync(int usuarioId, int roleId);

        // Operaciones de persistencia y autenticación
        Task SaveAsync();
        void Delete(Usuario usuario);
        bool ValidatePassword(Usuario usuario, string password);
    }
}
