using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class CargoService(ICargoRepository repo) : ICargoService
    {
        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(SaveCargoDto dto)
        {
            if (await repo.NombreExisteAsync(dto.Name.Trim()))
                return (false, "El nombre del cargo ya existe");

            await repo.CreateAsync(new Cargo
            {
                Name = dto.Name.Trim()
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (await repo.HasAnyEmpleadoAsync(id))
                return (false, "No se puede eliminar el cargo porque tiene empleados asignados");

            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "Cargo no encontrado");
        }

        public async Task<IReadOnlyList<CargoTableDto>> GetAllAsync()
        {
            var cargos = await repo.GetAllAsync();

            return cargos.Select(c => new CargoTableDto
            {
                Id = c.Id,
                Name = c.Name,
                TotalEmpleados = c.Empleados.Count
            }).ToList();
        }

        public async Task<SaveCargoDto?> GetByIdAsync(int id)
        {
            var cargo = await repo.GetByIdAsync(id);

            if (cargo == null)
                return null;

            return new SaveCargoDto
            {
                Id = cargo.Id,
                Name = cargo.Name
            };
        }

        public Task<DataTableResponseDto<CargoTableDto>> GetPagedAsync(DataTablesRequestDto request)
            => repo.GetPagedAsync(request);

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(SaveCargoDto dto)
        {
            if (!dto.Id.HasValue)
                return (false, "Id inválido");

            var cargo = await repo.GetByIdAsync(dto.Id.Value);

            if (cargo == null)
                return (false, "Cargo no encontrado");

            if (await repo.NombreExisteAsync(dto.Name.Trim(), dto.Id))
                return (false, "Ya existe un cargo con ese nombre");

            cargo.Name = dto.Name.Trim();

            await repo.UpdateAsync(cargo);

            return (true, null);
        }
    }
}
