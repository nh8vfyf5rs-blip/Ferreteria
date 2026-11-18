using Ferreteria.DTOS;
using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IVentaRepository
    {
        Task<DataTableResponseDto<VentaTableDto>> GetPagedAsync(DataTablesRequestDto request);
        Task<IReadOnlyList<Venta>> GetAllAsync();
        Task<Venta?> GetByIdAsync(int id);
        Task<Venta?> CreateAsync(Venta venta);
        Task<bool> UpdateAsync(Venta venta);
        Task<bool> DeleteAsync(int id);
    }
}
