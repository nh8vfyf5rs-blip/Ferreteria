using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IMarcaService
    {
        Task<IReadOnlyList<MarcaTableDto>> GetAllAsync();
        Task<MarcaDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateMarcaDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateMarcaDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
