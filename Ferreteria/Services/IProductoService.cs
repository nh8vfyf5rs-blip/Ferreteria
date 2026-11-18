using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IProductoService
    {
        Task<IReadOnlyList<ProductoTableDto>> GetAllAsync();

        // Consulta compatible: stock total de todos los almacenes.
        Task<ProductoDto?> GetByIdAsync(int id);

        // Nueva consulta: stock del almacén seleccionado.
        Task<ProductoDto?> GetByIdAsync(int id, int almacenId);

        Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateProductoDto dto);

        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateProductoDto dto);

        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
