using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class CargoController(ICargoService _cargoService) : Controller
    {
        [Authorize(Policy = "cargo.ver")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "cargo.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData(DataTablesRequestDto request)
        {
            var result = await _cargoService.GetPagedAsync(request);

            return Ok(result);
        }

        [Authorize(Policy = "cargo.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _cargoService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "Cargo no encontrado"
                });

            return Ok(result);
        }

        [Authorize(Policy = "cargo.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(SaveCargoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _cargoService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Cargo creado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }

        [Authorize(Policy = "cargo.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(SaveCargoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _cargoService.UpdateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Cargo actualizado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }

        [Authorize(Policy = "cargo.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _cargoService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Cargo eliminado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }
    }
}
