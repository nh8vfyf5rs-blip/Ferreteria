using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IMovimientoInventarioService
    {
        Task<IReadOnlyList<MovimientoInventarioTableDto>> GetAllAsync();
        Task<MovimientoInventarioDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateMovimientoInventarioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateMovimientoInventarioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
