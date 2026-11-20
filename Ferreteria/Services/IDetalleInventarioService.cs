using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IDetalleInventarioService
    {
        Task<IReadOnlyList<DetalleInventarioTableDto>> GetAllAsync();
        Task<DetalleInventarioDTO?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateDetalleInventarioDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateDetalleInventarioDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
