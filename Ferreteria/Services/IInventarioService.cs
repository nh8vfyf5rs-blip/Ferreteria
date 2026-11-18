using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IInventarioService
    {
        Task<IReadOnlyList<InventarioTableDto>> GetAllAsync();
        Task<InventarioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateInventarioDTO dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateInventarioDTO dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
