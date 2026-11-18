using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class EmpleadoController(
        IEmpleadoService _empleadoService,
        ICargoService _cargoService,
        ISucursalService _sucursalService) : Controller
    {
        [Authorize(Policy = "empleado.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Cargos = await _cargoService.GetAllAsync();
            ViewBag.Sucursales = await _sucursalService.GetAllAsync();

            return View();
        }

        [Authorize(Policy = "empleado.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _empleadoService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "empleado.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _empleadoService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Empleado no encontrado" });

            return Ok(result);
        }

        [Authorize(Policy = "empleado.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmpleadoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _empleadoService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Empleado creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "empleado.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(
            int id,
            UpdateEmpleadoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _empleadoService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Empleado actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "empleado.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _empleadoService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Empleado eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
