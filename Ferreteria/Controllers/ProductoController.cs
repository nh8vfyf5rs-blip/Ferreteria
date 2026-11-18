using Ferreteria.DTOS;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ferreteria.Controllers
{
    public class ProductoController(
        IProductoService _productoService,
        ICategoriaService _categoriaService,
        IMarcaService _marcaService,
        IAlmacenService _almacenService) : Controller
    {
        [Authorize(Policy = "producto.ver")]
        public async Task<IActionResult> Index()
        {
            await CargarListasAsync();
            return View();
        }

        [Authorize(Policy = "producto.ver")]
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await _productoService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "producto.ver")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id, int? almacenId = null)
        {
            ProductoDto? result;

            if (almacenId.HasValue)
            {
                if (almacenId.Value <= 0)
                    return BadRequest(new
                    {
                        message = "El almacén seleccionado no es válido."
                    });

                result = await _productoService.GetByIdAsync(
                    id, almacenId.Value);
            }
            else
            {
                result = await _productoService.GetByIdAsync(id);
            }

            if (result == null)
                return NotFound(new
                {
                    message = "Producto o almacén no encontrado."
                });

            return Ok(result);
        }

        [Authorize(Policy = "producto.crear")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _productoService.CreateAsync(dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Producto creado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "producto.actualizar")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    success = false,
                    message = "Error en los datos a guardar"
                });

            var (success, errorMessage) =
                await _productoService.UpdateAsync(id, dto);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Producto actualizado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        [Authorize(Policy = "producto.borrar")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errorMessage) =
                await _productoService.DeleteAsync(id);

            return success
                ? Ok(new
                {
                    success = true,
                    message = "Producto eliminado satisfactoriamente"
                })
                : BadRequest(new { success = false, errorMessage });
        }

        private async Task CargarListasAsync()
        {
            var categorias = await _categoriaService.GetAllAsync();
            var marcas = await _marcaService.GetAllAsync();
            var almacenes = await _almacenService.GetAllAsync();

            ViewBag.Categorias = new SelectList(
                categorias, "Id", "Nombre");

            ViewBag.Marcas = new SelectList(
                marcas, "Id", "Name");

            ViewBag.Almacenes = new SelectList(
                almacenes, "Id", "Name");
        }
    }
}

