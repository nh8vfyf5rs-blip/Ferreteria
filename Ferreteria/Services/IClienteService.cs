using Ferreteria.DTOS;

namespace Ferreteria.Services
{
    public interface IClienteService
    {
        Task<IReadOnlyList<ClienteTableDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync( CreateClienteDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id,UpdateClienteDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
    }
}
