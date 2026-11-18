using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class MetodoPagoController(IMetodoPagoService _metodoPagoService) : Controller
    {
        [Authorize(Policy = "metodopago.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "metodopago.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _metodoPagoService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "metodopago.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _metodoPagoService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Método de pago no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "metodopago.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMetodoPagoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _metodoPagoService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Método de pago creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "metodopago.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateMetodoPagoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _metodoPagoService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Método de pago actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "metodopago.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _metodoPagoService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Método de pago eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
