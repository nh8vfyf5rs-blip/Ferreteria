using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class CategoriaController(ICategoriaService _categoriaService) : Controller
    {
        [Authorize(Policy = "categoria.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "categoria.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _categoriaService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "categoria.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Categoría no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "categoria.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoriaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _categoriaService.CreateAsync(dto);

            return success
                ? Ok(new { success = true, message = "Categoría creada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "categoria.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateCategoriaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _categoriaService.UpdateAsync(dto);

            return success
                ? Ok(new { success = true, message = "Categoría actualizada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "categoria.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _categoriaService.DeleteAsync(id);

            return success
                ? Ok(new { success = true, message = "Categoría eliminada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }
    }
   
}
