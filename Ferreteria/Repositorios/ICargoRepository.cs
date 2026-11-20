using Ferreteria.DTOS;
using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ICargoRepository
    {
        Task<IReadOnlyList<Cargo>> GetAllAsync();
        Task<Cargo?> GetByIdAsync(int id);
        Task<Cargo?> CreateAsync(Cargo cargo);
        Task<bool> UpdateAsync(Cargo cargo);
        Task<bool> DeleteAsync(int id);
        Task<bool> NombreExisteAsync(string name, int? excludeId = null);
        Task<bool> HasAnyEmpleadoAsync(int cargoId);
        Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request);
    }
}