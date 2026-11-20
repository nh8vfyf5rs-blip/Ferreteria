using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface ISucursalService
    {
        Task<IReadOnlyList<SucursalTableDto>> GetAllAsync();
        Task<SucursalDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateSucursalDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateSucursalDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
