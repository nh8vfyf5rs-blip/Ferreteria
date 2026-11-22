using System.Security.Claims;
using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class DevolucionesController(IDevolucionesService devolucionesService) : Controller
    {
        private bool EsGerente()
        {
            return User?.IsInRole("Gerente") == true
                || User?.Claims.Any(c =>
                    (c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase) || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                    && string.Equals(c.Value, "Gerente", StringComparison.OrdinalIgnoreCase)) == true;
        }

        [Authorize(Policy = "devolucion.ver")]
        public IActionResult Index() => View();

        [Authorize(Policy = "devolucion.ver")]
        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var lista = await devolucionesService.GetAllAsync();
            return Json(new { data = lista });
        }

        [Authorize(Policy = "devolucion.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "ID inválido." });

            var devolucion = await devolucionesService.GetByIdAsync(id);
            if (devolucion == null)
                return Json(new { success = false, message = "Devolución no encontrada." });

            return Json(new { success = true, data = devolucion });
        }

        [Authorize(Policy = "devolucion.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateDevolucionDto dto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos." });

            var (success, errorMessage) = await devolucionesService.CreateAsync(dto);
            return Json(new
            {
                success,
                message = success ? "Devolución creada correctamente." : errorMessage
            });
        }

        [Authorize(Policy = "devolucion.actualizar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateDevolucionDto dto)
        {
            if (EsGerente())
                return Forbid();

            if (id <= 0 || !ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos para actualizar." });

            var (success, errorMessage) = await devolucionesService.UpdateAsync(id, dto);
            return Json(new
            {
                success,
                message = success ? "Devolución actualizada correctamente." : errorMessage
            });
        }

        [Authorize(Policy = "devolucion.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "ID inválido." });

            var (success, errorMessage) = await devolucionesService.DeleteAsync(id);
            return Json(new
            {
                success,
                message = success ? "Devolución eliminada correctamente." : errorMessage
            });
        }
    }

    public class DevolucionDto
    {
        public int Id { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int VentaId { get; set; }
    }
}