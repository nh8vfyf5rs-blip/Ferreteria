using Ferreteria.DTOS;
using Ferreteria.Models.Segurudad;

namespace Ferreteria.Services
{
    public interface IPermissionRepository
    {
 Task<DataTableResponseDto<PermisoDto>> GetPagedAsync(DataTablesRequestDto request);
    Task<IReadOnlyList<Permiso>> GetAllAsync();
    Task<Permiso?> GetByIdAsync(int Id);
    Task<Permiso?> GetByNameAsync(string Name);
    Task<Permiso> CreateAsync(string Name, string? description);
    Task<bool> UpdateAsync(Permiso permiso);
    Task<bool> DeleteAsync(int Id);
    Task<bool> IsAssingedToUsuarioAsync(int permisoId);
}
}
