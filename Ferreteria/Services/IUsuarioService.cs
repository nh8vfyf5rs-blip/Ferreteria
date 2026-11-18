using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IUsuarioService
    {
        Task<IReadOnlyList<UsuarioTableDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateUsuarioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateUsuarioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
