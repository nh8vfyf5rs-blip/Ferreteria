using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IProductoService
    {
        Task<IReadOnlyList<ProductoTableDto>> GetAllAsync();
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto?> GetByIdAsync(int id, int almacenId);
        Task<(bool Success, string? ErrorMessage)> CreateAsync( CreateProductoDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync( int id, UpdateProductoDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
