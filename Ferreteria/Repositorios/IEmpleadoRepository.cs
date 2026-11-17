using Ferreteria.Models;

namespace Ferreteria.Repositorios
{
    public interface IEmpleadoRepository
    {
        Task<IReadOnlyList<Empleado>> GetAllAsync();
        Task<Empleado?> GetByIdAsync(int id);
        Task<Empleado?> CreateAsync(Empleado empleado);
        Task<bool> UpdateAsync(Empleado empleado);
        Task<bool> DeleteAsync(int id);
    }
}
