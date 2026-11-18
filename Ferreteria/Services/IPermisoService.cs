using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IPermisoService
    {
        Task<IReadOnlyList<PermisoTableDto>> GetAllAsync();

        Task<PermisoDto?> GetByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreatePermisoDto dto);

        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdatePermisoDto dto);

        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
