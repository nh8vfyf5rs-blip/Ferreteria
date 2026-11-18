using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class InventarioController(
        IInventarioService _inventarioService,
        IAlmacenService _almacenService,
        IProductoService _productoService) : Controller
    {
        [Authorize(Policy = "inventario.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Almacenes = await _almacenService.GetAllAsync();
            ViewBag.Productos = await _productoService.GetAllAsync();

            return View();
        }

        [Authorize(Policy = "inventario.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _inventarioService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "inventario.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _inventarioService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Inventario no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "inventario.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateInventarioDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _inventarioService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Inventario creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "inventario.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(
            int id,
            UpdateInventarioDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _inventarioService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Inventario actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "inventario.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _inventarioService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Inventario eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
