using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface ICargoService
    {
        Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<CargoTableDto>> GetAllAsync();

        Task<SaveCargoDto?> GetByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCargoDto dto);

        Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCargoDto dto);

        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
