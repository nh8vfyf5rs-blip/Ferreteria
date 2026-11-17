using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class ClienteService(IClienteRepository repo) : IClienteService
    {
        public async Task<IReadOnlyList<ClienteTableDto>> GetAllAsync()
        {
            var clientes = await repo.GetAllAsync();

            return clientes.Select(cliente => new ClienteTableDto
            {
                Id = cliente.Id,
                Name = cliente.Name,
                Apellido = cliente.Apellido,
                Cedula = cliente.Cedula,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo,
                Direccion = cliente.Direccion
            }).ToList();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            var cliente = await repo.GetByIdAsync(id);

            if (cliente == null)
                return null;

            return new ClienteDto
            {
                Id = cliente.Id,
                Name = cliente.Name,
                Apellido = cliente.Apellido,
                Cedula = cliente.Cedula,
                Telefono = cliente.Telefono,
                Correo = cliente.Correo,
                Direccion = cliente.Direccion
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateClienteDto dto)
        {
            if (dto == null)
                return (false, "Los datos del cliente son obligatorios.");

            var nombre = dto.Name?.Trim();
            var apellido = dto.Apellido?.Trim();
            var cedula = dto.Cedula?.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(apellido))
                return (false, "El apellido del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(cedula))
                return (false, "La cédula del cliente es obligatoria.");

            if (await repo.CedulaExistsAsync(cedula))
                return (false, "Ya existe un cliente registrado con esa cédula.");

            var cliente = new Cliente
            {
                Name = nombre,
                Apellido = apellido,
                Cedula = cedula,
                Telefono = dto.Telefono?.Trim() ?? string.Empty,
                Correo = dto.Correo?.Trim() ?? string.Empty,
                Direccion = dto.Direccion?.Trim() ?? string.Empty,
                FechaRegistro = DateTime.Now
            };

            var creado = await repo.CreateAsync(cliente);

            if (creado == null)
                return (false, "No fue posible registrar el cliente.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id,
            UpdateClienteDto dto)
        {
            if (id <= 0)
                return (false, "El identificador del cliente no es válido.");

            if (dto == null)
                return (false, "Los datos del cliente son obligatorios.");

            var cliente = await repo.GetByIdAsync(id);

            if (cliente == null)
                return (false, "No se encontró el cliente solicitado.");

            var nombre = dto.Name?.Trim();
            var apellido = dto.Apellido?.Trim();
            var cedula = dto.Cedula?.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
                return (false, "El nombre del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(apellido))
                return (false, "El apellido del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(cedula))
                return (false, "La cédula del cliente es obligatoria.");

            if (await repo.CedulaExistsAsync(cedula, id))
                return (false, "Ya existe otro cliente registrado con esa cédula.");

            cliente.Name = nombre;
            cliente.Apellido = apellido;
            cliente.Cedula = cedula;
            cliente.Telefono = dto.Telefono?.Trim() ?? string.Empty;
            cliente.Correo = dto.Correo?.Trim() ?? string.Empty;
            cliente.Direccion = dto.Direccion?.Trim() ?? string.Empty;

            var actualizado = await repo.UpdateAsync(cliente);

            if (!actualizado)
                return (false, "No fue posible actualizar el cliente.");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            if (id <= 0)
                return (false, "El identificador del cliente no es válido.");

            var cliente = await repo.GetByIdAsync(id);

            if (cliente == null)
                return (false, "No se encontró el cliente solicitado.");

            if (await repo.HasVentasAsync(id))
            {
                return (
                    false,
                    "No se puede eliminar el cliente porque tiene ventas registradas."
                );
            }

            var eliminado = await repo.DeleteAsync(id);

            if (!eliminado)
                return (false, "No fue posible eliminar el cliente.");

            return (true, null);
        }
    }
}
