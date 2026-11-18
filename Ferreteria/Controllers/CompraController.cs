using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            ViewBag.Proveedores = await _proveedorService.GetAllAsync();
            ViewBag.Empleados = await _empleadoService.GetAllAsync();
            ViewBag.Almacenes = await _almacenService.GetAllAsync();
            ViewBag.Productos = await _productoService.GetAllAsync();

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
        public async Task<IActionResult> Create(CreateCompraDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _compraService.CreateAsync(dto);

            return success
                ? Ok(new { success = true, message = "Compra creada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "compra.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCompraDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) = await _compraService.UpdateAsync(id, dto);

            return success
                ? Ok(new { success = true, message = "Compra actualizada satisfactoriamente" })
                : BadRequest(new { success = false, errorMessage });
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
