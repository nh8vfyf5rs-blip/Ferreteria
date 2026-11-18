using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IProveedorService
    {
        Task<IReadOnlyList<ProveedorTableDto>> GetAllAsync();

        Task<ProveedorDto?> GetByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateProveedorDto dto);

        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateProveedorDto dto);

        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
