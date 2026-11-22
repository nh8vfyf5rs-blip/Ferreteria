using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ferreteria.Controllers
{
    public class CompraController(
        ICompraService _compraService,
        IProveedorService _proveedorService,
        IEmpleadoService _empleadoService,
        IAlmacenService _almacenService,
        IProductoService _productoService) : Controller
    {
        [Authorize(Policy = "compra.ver")]
        public async Task<IActionResult> Index()
        {
            var proveedores = await _proveedorService.GetAllAsync();
            var empleados = await _empleadoService.GetAllAsync();
            var almacenes = await _almacenService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();

            ViewBag.Proveedores = new SelectList(proveedores, "Id", "NombreEmpresa");
            ViewBag.Empleados = new SelectList(empleados, "Id", "Nombres");
            ViewBag.Almacenes = new SelectList(almacenes, "Id", "Name");
            ViewBag.Productos = new SelectList(productos, "Id", "Nombre");

            return View();
        }

        [Authorize(Policy = "compra.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _compraService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "compra.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _compraService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Compra no encontrada" });

            return Ok(result);
        }

        [Authorize(Policy = "compra.crear")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompraDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                var errorMessage = errors.Count > 0
                    ? string.Join(" | ", errors)
                    : "Error en los datos a guardar";

                return BadRequest(new { success = false, message = errorMessage, errorMessage });
            }

            var (success, errorMsg) = await _compraService.CreateAsync(dto);
            return success
                ? Ok(new { success = true, message = "Compra creada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage = errorMsg });
        }

        [Authorize(Policy = "compra.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCompraDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList();

                var errorMessage = errors.Count > 0
                    ? string.Join(" | ", errors)
                    : "Error en los datos a guardar";

                return BadRequest(new { success = false, message = errorMessage, errorMessage });
            }

            var (success, errorMsg) = await _compraService.UpdateAsync(id, dto);
            return success
                ? Ok(new { success = true, message = "Compra actualizada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage = errorMsg });
        }

        [Authorize(Policy = "compra.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) = await _compraService.DeleteAsync(id);
            return success
                ? Ok(new { success = true, message = "Compra eliminada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }
    }
}