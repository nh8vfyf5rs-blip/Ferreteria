using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IMetodoPagoService
    {
        Task<IReadOnlyList<MetodoPagoTableDto>> GetAllAsync();
        Task<MetodoPagoDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateMetodoPagoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateMetodoPagoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
