using Ferreteria.DTOS;
using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface ICargoRepository
    {
        Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request);

        Task<IReadOnlyList<Cargo>> GetAllAsync();

        Task<Cargo?> GetByIdAsync(int id);

        Task<bool> NombreExisteAsync(string nombre, int? excludeId = null);

        Task<Cargo?> CreateAsync(Cargo cargo);

        Task<bool> UpdateAsync(Cargo cargo);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasAnyEmpleadoAsync(int id);
    }
}