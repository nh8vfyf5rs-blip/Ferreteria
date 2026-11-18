using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class ClienteController(IClienteService _clienteService) : Controller
    {
        [Authorize(Policy = "cliente.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "cliente.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _clienteService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "cliente.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _clienteService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Cliente no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "cliente.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateClienteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _clienteService.CreateAsync(dto);

            return success
                ? Ok(new { success = true, message = "Cliente creado satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "cliente.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateClienteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _clienteService.UpdateAsync(id, dto);

            return success
                ? Ok(new { success = true, message = "Cliente actualizado satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "cliente.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _clienteService.DeleteAsync(id);

            return success
                ? Ok(new { success = true, message = "Cliente eliminado satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
