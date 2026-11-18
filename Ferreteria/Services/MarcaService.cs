using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

namespace Ferreteria.Services
{
    public class MarcaService(IMarcaRepository repo) : IMarcaService
    {
        public async Task<IReadOnlyList<MarcaTableDto>> GetAllAsync()
        {
            var marcas = await repo.GetAllAsync();

            return marcas.Select(m => new MarcaTableDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();
        }

        public async Task<MarcaDto?> GetByIdAsync(int id)
        {
            var marca = await repo.GetByIdAsync(id);

            if (marca == null)
                return null;

            return new MarcaDto
            {
                Id = marca.Id,
                Name = marca.Name
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateMarcaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre de la marca es obligatorio.");

            var marca = new Marca
            {
                Name = dto.Name.Trim()
            };

            await repo.CreateAsync(marca);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateMarcaDto dto)
        {
            var marca = await repo.GetByIdAsync(id);

            if (marca == null)
                return (false, "La marca no existe.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return (false, "El nombre de la marca es obligatorio.");

            marca.Name = dto.Name.Trim();

            await repo.UpdateAsync(marca);

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var marca = await repo.GetByIdAsync(id);

            if (marca == null)
                return (false, "La marca no existe.");

            if (marca.Productos.Any())
                return (false, "No se puede eliminar la marca porque tiene productos asociados.");

            await repo.DeleteAsync(id);

            return (true, null);
        }
    }
}
