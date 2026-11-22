using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Services
{
    public class ProductoService(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IMarcaRepository marcaRepository,
        IAlmacenRepository almacenRepository,
        IInventarioRepository inventarioRepository,
        IInventarioService inventarioService) : IProductoService
    {
        public async Task<IReadOnlyList<ProductoTableDto>> GetAllAsync()
        {
            var productos = await productoRepository.GetAllAsync();

            return productos.Select(producto => new ProductoTableDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                CategoriaNombre = producto.Categoria?.Name ?? string.Empty,
                MarcaNombre = producto.Marca?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await productoRepository.GetByIdAsync(id);

            if (producto == null)
                return null;

            var inventarios = await inventarioRepository.GetAllAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioVenta ?? producto.PrecioCompra,
                Stock = inventarios
                    .Where(i => i.ProductoId == producto.Id)
                    .Sum(i => i.Stock),
                CategoriaId = producto.CategoriaId,
                ProveedorId = 0
            };
        }

        public async Task<ProductoDto?> GetByIdAsync(int id, int almacenId)
        {
            var producto = await productoRepository.GetByIdAsync(id);

            if (producto == null)
                return null;

            var almacen = await almacenRepository.GetByIdAsync(almacenId);

            if (almacen == null)
                return null;

            var inventarios = await inventarioRepository.GetAllAsync();

            var inventario = inventarios.FirstOrDefault(i =>
                i.ProductoId == id &&
                i.AlmacenId == almacenId);

            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioVenta ?? producto.PrecioCompra,
                Stock = inventario?.Stock ?? 0,
                CategoriaId = producto.CategoriaId,
                ProveedorId = 0
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateProductoDto dto)
        {
            var categoria = await categoriaRepository.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                return (false, "La categoría seleccionada no existe.");

            var marcas = await marcaRepository.GetAllAsync();
            var marca = marcas.FirstOrDefault();
            if (marca == null)
                return (false, "No hay ninguna marca registrada en el sistema.");

            var almacenes = await almacenRepository.GetAllAsync();
            var almacen = almacenes.FirstOrDefault();
            if (almacen == null)
                return (false, "No hay ningún almacén registrado en el sistema.");

            var codigoGenerado = await GenerarCodigoUnicoAsync(dto.Nombre);

            var producto = new Producto
            {
                Codigo = codigoGenerado,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioCompra = dto.Precio,
                PrecioVenta = dto.Precio,
                CategoriaId = dto.CategoriaId,
                MarcaId = marca.Id
            };

            var productoCreado = await productoRepository.CreateAsync(producto);

            if (productoCreado == null)
                return (false, "No se pudo crear el producto.");

            var inventarioDto = new CreateInventarioDTO
            {
                ProductoId = productoCreado.Id,
                AlmacenId = almacen.Id,
                Stock = dto.Stock
            };

            var (inventarioCreado, errorInventario) =
                await inventarioService.CreateAsync(inventarioDto);

            if (!inventarioCreado)
            {
                return (false,
                    errorInventario ??
                    "El producto se creó, pero no se pudo registrar su inventario inicial.");
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            int id, UpdateProductoDto dto)
        {
            var producto = await productoRepository.GetByIdAsync(id);
            if (producto == null)
                return (false, "El producto no existe.");

            var categoria = await categoriaRepository.GetByIdAsync(dto.CategoriaId);
            if (categoria == null)
                return (false, "La categoría seleccionada no existe.");

            var almacenes = await almacenRepository.GetAllAsync();
            var almacen = almacenes.FirstOrDefault();
            if (almacen == null)
                return (false, "No hay ningún almacén registrado en el sistema.");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.PrecioVenta = dto.Precio;
            producto.CategoriaId = dto.CategoriaId;

            if (string.IsNullOrWhiteSpace(producto.Codigo))
            {
                producto.Codigo = await GenerarCodigoUnicoAsync(dto.Nombre);
            }

            var actualizado = await productoRepository.UpdateAsync(producto);

            if (!actualizado)
                return (false, "No se pudo actualizar el producto.");

            var inventarios = await inventarioRepository.GetAllAsync();
            var inventario = inventarios.FirstOrDefault(i =>
                i.ProductoId == id &&
                i.AlmacenId == almacen.Id);

            if (inventario == null)
            {
                var inventarioDto = new CreateInventarioDTO
                {
                    ProductoId = id,
                    AlmacenId = almacen.Id,
                    Stock = dto.Stock
                };

                var (creado, errorInventario) =
                    await inventarioService.CreateAsync(inventarioDto);

                if (!creado)
                {
                    return (false,
                        errorInventario ??
                        "El producto se actualizó, pero no se pudo registrar el inventario.");
                }
            }
            else
            {
                var inventarioDto = new UpdateInventarioDTO
                {
                    ProductoId = id,
                    AlmacenId = almacen.Id,
                    Stock = dto.Stock
                };

                var (inventarioActualizado, errorInventario) =
                    await inventarioService.UpdateAsync(
                        inventario.Id, inventarioDto);

                if (!inventarioActualizado)
                {
                    return (false,
                        errorInventario ??
                        "El producto se actualizó, pero no se pudo actualizar el inventario.");
                }
            }

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var producto = await productoRepository.GetByIdAsync(id);

            if (producto == null)
                return (false, "El producto no existe.");

            if (producto.DetalleVentas.Count > 0)
                return (false, "No se puede eliminar porque tiene registros de ventas asociados.");

            if (producto.DetalleCompras.Count > 0)
                return (false, "No se puede eliminar porque tiene registros de compras asociados.");

            if (producto.DetalleInventarios.Count > 0)
                return (false, "No se puede eliminar porque tiene movimientos de inventario asociados.");

            var inventarios = await inventarioRepository.GetAllAsync();
            var inventariosProducto = inventarios.Where(i => i.ProductoId == id).ToList();

            if (inventariosProducto.Any(i => i.Stock > 0))
                return (false, "No se puede eliminar porque el producto aún tiene existencias en inventario.");

            try
            {
                foreach (var inv in inventariosProducto)
                {
                    await inventarioRepository.DeleteAsync(inv.Id);
                }

                var eliminado = await productoRepository.DeleteAsync(id);

                if (!eliminado)
                    return (false, "No se pudo eliminar el producto.");

                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se puede eliminar el producto porque tiene registros relacionados en el sistema.");
            }
        }

        private async Task<string> GenerarCodigoUnicoAsync(string nombre)
        {
            var productos = await productoRepository.GetAllAsync();
            var prefijo = new string((nombre ?? "PRD")
                .Where(char.IsLetterOrDigit)
                .Take(3)
                .ToArray())
                .ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(prefijo))
                prefijo = "PRD";

            string codigo;
            do
            {
                codigo = $"{prefijo}-{Random.Shared.Next(1000, 9999)}";
            } while (productos.Any(p =>
                string.Equals(p.Codigo, codigo, StringComparison.OrdinalIgnoreCase)));

            return codigo;
        }
    }
}