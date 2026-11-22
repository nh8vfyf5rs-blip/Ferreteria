using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class VentaController(
      IVentaService _ventaService,
      IClienteService _clienteService,
      IEmpleadoService _empleadoService,
      IAlmacenService _almacenService,
      IProductoService _productoService,
      IMetodoPagoService _metodoPagoService) : Controller
    {
        [Authorize(Policy = "venta.ver")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Clientes = await _clienteService.GetAllAsync();
            ViewBag.Empleados = await _empleadoService.GetAllAsync();
            ViewBag.Almacenes = await _almacenService.GetAllAsync();
            ViewBag.Productos = await _productoService.GetAllAsync();
            ViewBag.MetodosPago = await _metodoPagoService.GetAllAsync();

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
        public async Task<IActionResult> Create([FromBody] SaveVentaDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar",
                    errorMessage = string.Join(" | ", errors),
                    errors
                });
            }

            try
            {
                var (success, errorMessage) = await _ventaService.CreateAsync(dto);

                return success
                    ? Ok(new
                    {
                        success = true,
                        message = "Venta creada satisfactoriamente"
                    })
                    : BadRequest(new { success = false, errorMessage, message = errorMessage });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new
                {
                    success = false,
                    errorMessage = detail,
                    message = detail
                });
            }
        }

        [Authorize(Policy = "venta.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SaveVentaDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar",
                    errorMessage = string.Join(" | ", errors),
                    errors
                });
            }

            try
            {
                var (success, errorMessage) = await _ventaService.UpdateAsync(dto);

                return success
                    ? Ok(new
                    {
                        success = true,
                        message = "Venta actualizada satisfactoriamente"
                    })
                    : BadRequest(new { success = false, errorMessage, message = errorMessage });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new
                {
                    success = false,
                    errorMessage = detail,
                    message = detail
                });
            }
        }

        [Authorize(Policy = "venta.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var (success, errorMessage) = await _ventaService.DeleteAsync(id);

                return success
                    ? Ok(new
                    {
                        success = true,
                        message = "Venta eliminada satisfactoriamente"
                    })
                    : BadRequest(new { success = false, errorMessage, message = errorMessage });
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new
                {
                    success = false,
                    errorMessage = detail,
                    message = detail
                });
            }
        }
    }
}