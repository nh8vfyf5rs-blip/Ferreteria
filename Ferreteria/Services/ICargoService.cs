using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface ICargoService
    {
        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCargoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
        Task<IReadOnlyList<CargoTableDto>> GetAllAsync();
        Task<SaveCargoDto?> GetByIdAsync(int id);
        Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCargoDto dto);
    }
}
