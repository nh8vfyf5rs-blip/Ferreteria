using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IVentaService
    {
        Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<VentaTableDto>> GetAllAsync();

        Task<SaveVentaDto?> GetByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveVentaDto dto);

        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveVentaDto dto);

        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
