using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IDevolucionesService
    {
        Task<IReadOnlyList<DevolucionTableDto>> GetAllAsync();
        Task<DevolucionDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync( CreateDevolucionDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateDevolucionDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
