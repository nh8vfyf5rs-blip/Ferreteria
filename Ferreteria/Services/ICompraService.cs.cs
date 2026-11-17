using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface ICompraService
    {
        Task<IReadOnlyList<CompraTableDto>> GetAllAsync();
        Task<CompraDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateCompraDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id,
            UpdateCompraDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
