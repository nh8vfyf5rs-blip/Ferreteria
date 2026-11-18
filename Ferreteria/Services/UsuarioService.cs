using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class UsuarioService(
        IUsuarioRepository repo,
        IRolRepository repoRol) : IUsuarioService
    {
        public async Task<IReadOnlyList<UsuarioTableDto>> GetAllAsync()
        {
            var usuarios = await repo.GetAllAsync();

            return usuarios.Select(u => new UsuarioTableDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                EmailConfirmed = u.EmailConfirmed,
                RolId = u.RolId
            }).ToList();
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await repo.GetByIdAsync(id);

            if (usuario == null)
                return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                UserName = usuario.UserName,
                Password = string.Empty,
                Email = usuario.Email,
                EmailConfirmed = usuario.EmailConfirmed,
                RolId = usuario.RolId
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return (false, "El nombre de usuario es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return (false, "El correo electrónico es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return (false, "La contraseña es obligatoria");

            var usuarios = await repo.GetAllAsync();

            if (usuarios.Any(u =>
                string.Equals(u.UserName, dto.UserName.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
                return (false, "El nombre de usuario ya está registrado");

            if (usuarios.Any(u =>
                string.Equals(u.Email, dto.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
                return (false, "El correo electrónico ya está registrado");

            if (await repoRol.GetByIdAsync(dto.RolId) == null)
                return (false, "El rol seleccionado no existe");

            await repo.CreateAsync(new Usuario
            {
                UserName = dto.UserName.Trim(),
                Email = dto.Email.Trim(),
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password),
                RolId = dto.RolId,
                EmailConfirmed = false
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
                return (false, "El nombre de usuario es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return (false, "El correo electrónico es obligatorio");

            var usuario = await repo.GetByIdAsync(id);

            if (usuario == null)
                return (false, "Usuario no encontrado");

            var usuarios = await repo.GetAllAsync();

            if (usuarios.Any(u =>
                u.Id != id &&
                string.Equals(u.UserName, dto.UserName.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
                return (false, "El nombre de usuario ya está registrado");

            if (usuarios.Any(u =>
                u.Id != id &&
                string.Equals(u.Email, dto.Email.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
                return (false, "El correo electrónico ya está registrado");

            if (await repoRol.GetByIdAsync(dto.RolId) == null)
                return (false, "El rol seleccionado no existe");

            usuario.UserName = dto.UserName.Trim();
            usuario.Email = dto.Email.Trim();
            usuario.RolId = dto.RolId;

            await repo.UpdateAsync(usuario);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "Usuario no encontrado");
        }
    }
}
