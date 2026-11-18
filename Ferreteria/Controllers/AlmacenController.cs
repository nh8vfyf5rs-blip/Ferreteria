using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class AlmacenController(
        IAlmacenService _almacenService,
        ISucursalService _sucursalService) : Controller
    {
        [Authorize(Policy = "almacen.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Sucursales = await _sucursalService.GetAllAsync();

            return View();
        }

        [Authorize(Policy = "almacen.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _almacenService.GetAllAsync();

            return Ok(result);
        }

        [Authorize(Policy = "almacen.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _almacenService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new
                {
                    message = "Almacén no encontrado"
                });

            return Ok(result);
        }

        [Authorize(Policy = "almacen.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAlmacenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _almacenService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Almacén creado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }

        [Authorize(Policy = "almacen.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateAlmacenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _almacenService.UpdateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Almacén actualizado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }

        [Authorize(Policy = "almacen.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _almacenService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Almacén eliminado satisfactoriamente"
                })
                : BadRequest(new
                {
                    success = false,
                    errorMessage
                });
        }
    }
}
