using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ferreteria.Controllers
{
    public class VentaController(
        IVentaService _ventaService,
        IClienteService _clienteService,
        IEmpleadoService _empleadoService,
        IAlmacenService _almacenService,
        IProductoService _productoService) : Controller
    {
        [Authorize(Policy = "venta.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Clientes = await _clienteService.GetAllAsync();
            ViewBag.Empleados = await _empleadoService.GetAllAsync();
            ViewBag.Almacenes = await _almacenService.GetAllAsync();
            ViewBag.Productos = await _productoService.GetAllAsync();

            return View();
        }

        [Authorize(Policy = "venta.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData(DataTablesRequestDto request)
        {
            var result = await _ventaService.GetPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "venta.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _ventaService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Venta no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "venta.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(SaveVentaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _ventaService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Venta creada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "venta.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(SaveVentaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _ventaService.UpdateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Venta actualizada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "venta.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _ventaService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Venta eliminada satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}
