using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class ProveedorController(IProveedorService _proveedorService) : Controller
    {
        [Authorize(Policy = "proveedor.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "proveedor.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _proveedorService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "proveedor.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _proveedorService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Proveedor no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "proveedor.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProveedorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _proveedorService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Proveedor creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "proveedor.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProveedorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _proveedorService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Proveedor actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "proveedor.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _proveedorService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Proveedor eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
