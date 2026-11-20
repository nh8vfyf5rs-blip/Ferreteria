using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class DetalleInventarioController(
        IDetalleInventarioService _movimientoInventarioService,
        IInventarioService _inventarioService) : Controller
    {
        [Authorize(Policy = "movimientoinventario.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Inventarios = await _inventarioService.GetAllAsync();
            return View();
        }

        [Authorize(Policy = "movimientoinventario.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _movimientoInventarioService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "movimientoinventario.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _movimientoInventarioService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Movimiento de inventario no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "movimientoinventario.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDetalleInventarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _movimientoInventarioService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Movimiento de inventario creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "movimientoinventario.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateDetalleInventarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _movimientoInventarioService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Movimiento de inventario actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "movimientoinventario.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _movimientoInventarioService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Movimiento de inventario eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}