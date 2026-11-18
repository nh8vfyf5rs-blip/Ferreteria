using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class SucursalController(ISucursalService _sucursalService) : Controller
    {
        [Authorize(Policy = "sucursal.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "sucursal.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _sucursalService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "sucursal.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sucursalService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Sucursal no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "sucursal.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSucursalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _sucursalService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Sucursal creada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "sucursal.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSucursalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _sucursalService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Sucursal actualizada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "sucursal.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _sucursalService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Sucursal eliminada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
