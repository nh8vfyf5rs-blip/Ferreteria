using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IAlmacenService
    {
        Task<IReadOnlyList<AlmacenTableDto>> GetAllAsync();
        Task<UpdateAlmacenDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateAlmacenDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(UpdateAlmacenDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
  
