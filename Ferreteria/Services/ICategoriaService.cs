using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface ICategoriaService
    {
        Task<IReadOnlyList<CategoriaTableDto>> GetAllAsync();
        Task<UpdateCategoriaDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(    CreateCategoriaDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync( UpdateCategoriaDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
