using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class DevolucionController(
        IDevolucionesService _devolucionesService,
        IVentaService _ventaService) : Controller
    {
        [Authorize(Policy = "devolucion.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Ventas = await _ventaService.GetAllAsync();
            return View();
        }

        [Authorize(Policy = "devolucion.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _devolucionesService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "devolucion.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _devolucionesService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Devolución no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "devolucion.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDevolucionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _devolucionesService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Devolución creada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "devolucion.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(
            int id,
            UpdateDevolucionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _devolucionesService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Devolución actualizada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "devolucion.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _devolucionesService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Devolución eliminada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
