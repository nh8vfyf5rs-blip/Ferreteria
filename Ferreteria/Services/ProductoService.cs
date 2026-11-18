using Ferreteria.DTOS;
using Ferreteria.Models;
using Ferreteria.Repositorios;

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
            var inventarios = await inventarioRepository.GetAllAsync();

            return productos.Select(producto => new ProductoTableDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioVenta ?? 0,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Stock = inventarios
                    .Where(i => i.ProductoId == producto.Id)
                    .Sum(i => i.Stock),
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = producto.Categoria.Name,
                MarcaId = producto.MarcaId,
                MarcaNombre = producto.Marca.Name
            }).ToList();
        }

        // Consulta compatible: devuelve el stock total de todos los almacenes.
        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await productoRepository.GetByIdAsync(id);

            if (producto == null)
                return null;

            var inventarios = await inventarioRepository.GetAllAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioVenta ?? 0,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Stock = inventarios
                    .Where(i => i.ProductoId == producto.Id)
                    .Sum(i => i.Stock),
                CategoriaId = producto.CategoriaId,
                MarcaId = producto.MarcaId,

                // Se conserva por compatibilidad.
                // Producto no tiene una relación directa con Proveedor.
                ProveedorId = 0
            };
        }

        // Nueva consulta: devuelve el stock del almacén seleccionado.
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
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.PrecioVenta ?? 0,
                PrecioCompra = producto.PrecioCompra,
                PrecioVenta = producto.PrecioVenta,
                Stock = inventario?.Stock ?? 0,
                AlmacenId = almacenId,
                CategoriaId = producto.CategoriaId,
                MarcaId = producto.MarcaId,

                // Se conserva por compatibilidad.
                // Producto no tiene una relación directa con Proveedor.
                ProveedorId = 0
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            CreateProductoDto dto)
        {
            var productos = await productoRepository.GetAllAsync();

            if (productos.Any(p =>
                p.Codigo.Equals(dto.Codigo, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Ya existe un producto con ese código.");
            }

            var categoria = await categoriaRepository.GetByIdAsync(dto.CategoriaId);

            if (categoria == null)
                return (false, "La categoría seleccionada no existe.");

            var marca = await marcaRepository.GetByIdAsync(dto.MarcaId);

            if (marca == null)
                return (false, "La marca seleccionada no existe.");

            var almacen = await almacenRepository.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta ?? dto.Precio,
                CategoriaId = dto.CategoriaId,
                MarcaId = dto.MarcaId
            };

            var productoCreado = await productoRepository.CreateAsync(producto);

            if (productoCreado == null)
                return (false, "No se pudo crear el producto.");

            // El inventario se registra mediante InventarioService
            // para generar también su historial y movimiento.
            var inventarioDto = new CreateInventarioDTO
            {
                ProductoId = productoCreado.Id,
                AlmacenId = dto.AlmacenId,
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

            var productos = await productoRepository.GetAllAsync();

            if (productos.Any(p =>
                p.Id != id &&
                p.Codigo.Equals(dto.Codigo, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Ya existe otro producto con ese código.");
            }

            var categoria = await categoriaRepository.GetByIdAsync(dto.CategoriaId);

            if (categoria == null)
                return (false, "La categoría seleccionada no existe.");

            var marca = await marcaRepository.GetByIdAsync(dto.MarcaId);

            if (marca == null)
                return (false, "La marca seleccionada no existe.");

            var almacen = await almacenRepository.GetByIdAsync(dto.AlmacenId);

            if (almacen == null)
                return (false, "El almacén seleccionado no existe.");

            producto.Codigo = dto.Codigo;
            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.PrecioCompra = dto.PrecioCompra;
            producto.PrecioVenta = dto.PrecioVenta ?? dto.Precio;
            producto.CategoriaId = dto.CategoriaId;
            producto.MarcaId = dto.MarcaId;

            var actualizado = await productoRepository.UpdateAsync(producto);

            if (!actualizado)
                return (false, "No se pudo actualizar el producto.");

            // Buscamos el inventario correspondiente al producto
            // y al almacén seleccionado.
            var inventarios = await inventarioRepository.GetAllAsync();

            var inventario = inventarios.FirstOrDefault(i =>
                i.ProductoId == id &&
                i.AlmacenId == dto.AlmacenId);

            if (inventario == null)
            {
                // Si no existe, se registra mediante InventarioService.
                var inventarioDto = new CreateInventarioDTO
                {
                    ProductoId = id,
                    AlmacenId = dto.AlmacenId,
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
                // Si existe, se actualiza mediante InventarioService
                // para conservar el historial y registrar el movimiento.
                var inventarioDto = new UpdateInventarioDTO
                {
                    ProductoId = id,
                    AlmacenId = dto.AlmacenId,
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

            if (inventarios.Any(i => i.ProductoId == id))
                return (false, "No se puede eliminar porque el producto tiene registros de inventario asociados.");

            var eliminado = await productoRepository.DeleteAsync(id);

            if (!eliminado)
                return (false, "No se pudo eliminar el producto.");

            return (true, null);
        }
    }
}
