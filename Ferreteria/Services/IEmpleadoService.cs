using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IEmpleadoService
    {
        Task<IReadOnlyList<EmpleadoTableDto>> GetAllAsync();
        Task<EmpleadoDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateEmpleadoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, UpdateEmpleadoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
