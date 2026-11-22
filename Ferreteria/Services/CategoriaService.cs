using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class CategoriaService(ICategoriaRepository repo) : ICategoriaService
    {
        public async Task<IReadOnlyList<CategoriaTableDto>> GetAllAsync()
        {
            var categorias = await repo.GetAllAsync();

            return categorias.Select(c => new CategoriaTableDto
            {
                Id = c.Id,
                Name = c.Name,
                TotalProductos = c.Productos?.Count ?? 0
            }).ToList();
        }

        public async Task<UpdateCategoriaDto?> GetByIdAsync(int id)
        {
            var categoria = await repo.GetByIdAsync(id);

            if (categoria == null)
                return null;

            return new UpdateCategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Name
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (false, "El nombre de la categoría es obligatorio.");

            await repo.CreateAsync(new Categoria
            {
                Name = dto.Nombre.Trim()
            });

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            UpdateCategoriaDto dto)
        {
            if (!dto.Id.HasValue || dto.Id.Value <= 0)
                return (false, "El identificador de la categoría no es válido.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (false, "El nombre de la categoría es obligatorio.");

            var categoria = await repo.GetByIdAsync(dto.Id.Value);

            if (categoria == null)
                return (false, "Categoría no encontrada.");

            categoria.Name = dto.Nombre.Trim();

            var updated = await repo.UpdateAsync(categoria);

            return updated
                ? (true, null)
                : (false, "No fue posible actualizar la categoría.");
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var categoria = await repo.GetByIdAsync(id);

            if (categoria == null)
                return (false, "Categoría no encontrada.");

            var tieneProductos = await repo.HasProductosAsync(id);

            if (tieneProductos)
                return (false, "No se puede eliminar la categoría porque tiene productos asociados.");

            var deleted = await repo.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, "No fue posible eliminar la categoría.");
        }
    }
}
