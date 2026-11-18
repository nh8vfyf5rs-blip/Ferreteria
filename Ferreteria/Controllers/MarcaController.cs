using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class MarcaController(IMarcaService _marcaService) : Controller
    {
        [Authorize(Policy = "marca.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "marca.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _marcaService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "marca.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _marcaService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Marca no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "marca.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMarcaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _marcaService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Marca creada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "marca.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateMarcaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _marcaService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Marca actualizada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "marca.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _marcaService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Marca eliminada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
