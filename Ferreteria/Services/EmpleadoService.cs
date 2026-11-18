using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class EmpleadoService(
        IEmpleadoRepository repo,
        ICargoRepository repoCargo,
        ISucursalRepository repoSucursal) : IEmpleadoService
    {
        public async Task<IReadOnlyList<EmpleadoTableDto>> GetAllAsync()
        {
            var empleados = await repo.GetAllAsync();

            return empleados.Select(e => new EmpleadoTableDto
            {
                Id = e.Id,
                Nombre = e.Nombres,
                Apellidos = e.Apellidos,
                Cedula = e.Cedula,
                Telefono = e.Telefono,
                Correo = e.Correo,
                Direccion = e.Direccion,
                FechaContratacion = e.FechaContratacion,
                Salario = e.Salario
            }).ToList();
        }

        public async Task<EmpleadoDto?> GetByIdAsync(int id)
        {
            var empleado = await repo.GetByIdAsync(id);

            if (empleado == null)
                return null;

            return new EmpleadoDto
            {
                Id = empleado.Id,
                Nombre = empleado.Nombres,
                Apellidos = empleado.Apellidos,
                Cedula = empleado.Cedula,
                Telefono = empleado.Telefono,
                Correo = empleado.Correo,
                Direccion = empleado.Direccion,
                FechaContratacion = empleado.FechaContratacion,
                Salario = empleado.Salario,
                CargoId = empleado.CargoId,
                SucursalId = empleado.SucursalId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateEmpleadoDto dto)
        {
            if (await repoCargo.GetByIdAsync(dto.CargoId) == null)
                return (false, "El cargo seleccionado no existe.");

            if (await repoSucursal.GetByIdAsync(dto.SucursalId) == null)
                return (false, "La sucursal seleccionada no existe.");

            var empleado = new Empleado
            {
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Cedula = dto.Cedula.Trim(),
                Telefono = dto.Telefono.Trim(),
                Correo = dto.Correo.Trim(),
                Direccion = dto.Direccion.Trim(),
                FechaContratacion = dto.FechaContratacion,
                Salario = dto.Salario,
                CargoId = dto.CargoId,
                SucursalId = dto.SucursalId
            };

            await repo.CreateAsync(empleado);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateEmpleadoDto dto)
        {
            var empleado = await repo.GetByIdAsync(id);

            if (empleado == null)
                return (false, "El empleado no existe.");

            if (await repoCargo.GetByIdAsync(dto.CargoId) == null)
                return (false, "El cargo seleccionado no existe.");

            if (await repoSucursal.GetByIdAsync(dto.SucursalId) == null)
                return (false, "La sucursal seleccionada no existe.");

            empleado.Nombres = dto.Nombres.Trim();
            empleado.Apellidos = dto.Apellidos.Trim();
            empleado.Cedula = dto.Cedula.Trim();
            empleado.Telefono = dto.Telefono.Trim();
            empleado.Correo = dto.Correo.Trim();
            empleado.Direccion = dto.Direccion.Trim();
            empleado.FechaContratacion = dto.FechaContratacion;
            empleado.Salario = dto.Salario;
            empleado.CargoId = dto.CargoId;
            empleado.SucursalId = dto.SucursalId;

            await repo.UpdateAsync(empleado);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var empleado = await repo.GetByIdAsync(id);

            if (empleado == null)
                return (false, "El empleado no existe.");

            if (await repo.HasComprasAsync(id))
                return (false, "No se puede eliminar el empleado porque tiene compras asociadas.");

            await repo.DeleteAsync(id);

            return (true, null);
        }
    }
}
