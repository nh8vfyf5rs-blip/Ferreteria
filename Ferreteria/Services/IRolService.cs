using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IRolService
    {
        Task<IReadOnlyList<RolTableDto>> GetAllAsync();
        Task<RolDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateRolDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateRolDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
