using Ferreteria.Models;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context);
            await SeedPermissionsAsync(context);
            await SeedRolePermissionsAsync(context);
            await SeedUsersAsync(context);
            await SeedCategoriasAsync(context);
            await SeedMarcasAsync(context);
            await SeedMetodosPagosAsync(context);
            await SeedCargosAsync(context);
        }

        private static async Task SeedCargosAsync(AppDbContext context)
        {
            var cargosIniciales = new[] { "Vendedor", "Cajero", "Bodeguero", "Gerente", "Administrador" };
            var changed = false;

            foreach (var cargoName in cargosIniciales)
            {
                if (!await context.Cargos.AnyAsync(c => c.Name == cargoName))
                {
                    context.Cargos.Add(new Cargo { Name = cargoName });
                    changed = true;
                }
            }

            if (changed)
                await context.SaveChangesAsync();
        }

        private static async Task SeedMarcasAsync(AppDbContext context)
        {
            if (await context.Marcas.AnyAsync())
                return;

            context.Marcas.AddRange(
                new Marca { Name = "Truper" },
                new Marca { Name = "Stanley" },
                new Marca { Name = "Cemex" },
                new Marca { Name = "PPR" }
            );
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriasAsync(AppDbContext context)
        {
            if (await context.Categorias.AnyAsync())
                return;

            context.Categorias.AddRange(
                new Categoria { Name = "Construccion" },
                new Categoria { Name = "Herramientas Manuales" },
                new Categoria { Name = "Fontaneria" },
                new Categoria { Name = "Electricidad" }
            );

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(AppDbContext context)
        {
            var rolesIniciales = new[] { "Admin", "Cajero", "Gerente", "Bodeguero" };
            var changed = false;

            foreach (var roleName in rolesIniciales)
            {
                if (!await context.Roles.AnyAsync(r => r.Name == roleName))
                {
                    context.Roles.Add(new Rol { Name = roleName });
                    changed = true;
                }
            }

            if (changed)
                await context.SaveChangesAsync();
        }

        private static async Task SeedPermissionsAsync(AppDbContext context)
        {
            var permisos = new List<Permiso>
            {
                new() { Name = "admin.panel", Descripcion = "Acceso al panel administrativo" },

                new() { Name = "permiso.ver", Descripcion = "Ver permisos" },
                new() { Name = "permiso.crear", Descripcion = "Crear permiso" },
                new() { Name = "permiso.actualizar", Descripcion = "Actualizar permiso" },
                new() { Name = "permiso.borrar", Descripcion = "Borrar permiso" },

                new() { Name = "rol.ver", Descripcion = "Ver roles del sistema" },
                new() { Name = "rol.crear", Descripcion = "Crear rol" },
                new() { Name = "rol.asignar", Descripcion = "Asignar permisos a roles" },

                new() { Name = "usuario.ver", Descripcion = "Ver usuarios del sistema" },
                new() { Name = "usuario.asignar", Descripcion = "Asignar roles y permisos a usuarios" },
                new() { Name = "usuario.cambiar_rol", Descripcion = "Cambiar rol de usuario" },

                new() { Name = "producto.ver", Descripcion = "Ver listado de productos" },
                new() { Name = "producto.crear", Descripcion = "Crear nuevo producto" },
                new() { Name = "producto.actualizar", Descripcion = "Actualizar producto" },
                new() { Name = "producto.borrar", Descripcion = "Eliminar producto" },

                new() { Name = "categoria.ver", Descripcion = "Ver categorías" },
                new() { Name = "categoria.crear", Descripcion = "Crear categoría" },
                new() { Name = "categoria.actualizar", Descripcion = "Actualizar categoría" },
                new() { Name = "categoria.borrar", Descripcion = "Eliminar categoría" },

                new() { Name = "marca.ver", Descripcion = "Ver marcas" },
                new() { Name = "marca.crear", Descripcion = "Crear marca" },
                new() { Name = "marca.actualizar", Descripcion = "Actualizar marca" },
                new() { Name = "marca.borrar", Descripcion = "Eliminar marca" },

                new() { Name = "proveedor.ver", Descripcion = "Ver proveedores" },
                new() { Name = "proveedor.crear", Descripcion = "Crear proveedor" },
                new() { Name = "proveedor.actualizar", Descripcion = "Actualizar proveedor" },
                new() { Name = "proveedor.borrar", Descripcion = "Eliminar proveedor" },

                new() { Name = "cliente.ver", Descripcion = "Ver clientes" },
                new() { Name = "cliente.crear", Descripcion = "Crear cliente" },
                new() { Name = "cliente.actualizar", Descripcion = "Actualizar cliente" },
                new() { Name = "cliente.borrar", Descripcion = "Eliminar cliente" },
                new() { Name = "cliente.gestionar", Descripcion = "Gestionar clientes" },

                new() { Name = "empleado.ver", Descripcion = "Ver empleados" },
                new() { Name = "empleado.crear", Descripcion = "Crear empleado" },
                new() { Name = "empleado.actualizar", Descripcion = "Actualizar empleado" },
                new() { Name = "empleado.borrar", Descripcion = "Eliminar empleado" },

                new() { Name = "cargo.ver", Descripcion = "Ver cargos" },
                new() { Name = "cargo.crear", Descripcion = "Crear cargo" },
                new() { Name = "cargo.actualizar", Descripcion = "Actualizar cargo" },
                new() { Name = "cargo.borrar", Descripcion = "Eliminar cargo" },

                new() { Name = "almacen.ver", Descripcion = "Ver almacenes" },
                new() { Name = "almacen.crear", Descripcion = "Crear almacén" },
                new() { Name = "almacen.actualizar", Descripcion = "Actualizar almacén" },
                new() { Name = "almacen.borrar", Descripcion = "Eliminar almacén" },

                new() { Name = "sucursal.ver", Descripcion = "Ver sucursales" },
                new() { Name = "sucursal.crear", Descripcion = "Crear sucursal" },
                new() { Name = "sucursal.actualizar", Descripcion = "Actualizar sucursal" },
                new() { Name = "sucursal.borrar", Descripcion = "Eliminar sucursal" },

                new() { Name = "inventario.ver", Descripcion = "Ver inventario" },
                new() { Name = "inventario.crear", Descripcion = "Crear inventario" },
                new() { Name = "inventario.actualizar", Descripcion = "Actualizar stock" },
                new() { Name = "inventario.borrar", Descripcion = "Eliminar inventario" },

                new() { Name = "movimientoinventario.ver", Descripcion = "Ver movimientos de inventario" },
                new() { Name = "movimientoinventario.crear", Descripcion = "Crear movimiento de inventario" },
                new() { Name = "movimientoinventario.actualizar", Descripcion = "Actualizar movimiento de inventario" },
                new() { Name = "movimientoinventario.borrar", Descripcion = "Eliminar movimiento de inventario" },

                new() { Name = "compra.ver", Descripcion = "Ver compras a proveedores" },
                new() { Name = "compra.crear", Descripcion = "Registrar compras de mercadería" },
                new() { Name = "compra.actualizar", Descripcion = "Actualizar compra" },
                new() { Name = "compra.borrar", Descripcion = "Eliminar compra" },

                new() { Name = "venta.ver", Descripcion = "Ver ventas realizadas" },
                new() { Name = "venta.crear", Descripcion = "Registrar nueva venta" },
                new() { Name = "venta.actualizar", Descripcion = "Actualizar venta" },
                new() { Name = "venta.borrar", Descripcion = "Eliminar venta" },

                new() { Name = "devolucion.ver", Descripcion = "Ver devoluciones" },
                new() { Name = "devolucion.crear", Descripcion = "Crear devolución" },
                new() { Name = "devolucion.actualizar", Descripcion = "Actualizar devolución" },
                new() { Name = "devolucion.borrar", Descripcion = "Eliminar devolución" },

                new() { Name = "metodopago.ver", Descripcion = "Ver métodos de pago" },
                new() { Name = "metodopago.crear", Descripcion = "Crear método de pago" },
                new() { Name = "metodopago.actualizar", Descripcion = "Actualizar método de pago" },
                new() { Name = "metodopago.borrar", Descripcion = "Eliminar método de pago" }
            };

            var existingNames = await context.Permisos
                .Select(p => p.Name)
                .ToListAsync();

            var existingSet = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);
            var nuevosPermisos = permisos.Where(p => !existingSet.Contains(p.Name)).ToList();

            if (nuevosPermisos.Count > 0)
            {
                context.Permisos.AddRange(nuevosPermisos);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRolePermissionsAsync(AppDbContext context)
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var cajeroRole = await context.Roles.FirstAsync(r => r.Name == "Cajero");
            var gerenteRole = await context.Roles.FirstAsync(r => r.Name == "Gerente");
            var bodegueroRole = await context.Roles.FirstAsync(r => r.Name == "Bodeguero");

            var permissions = await context.Permisos.ToListAsync();

            async Task GrantPermissionsToRoleAsync(int rolId, IEnumerable<Permiso> targetPermissions)
            {
                foreach (var permission in targetPermissions)
                {
                    var existing = await context.RolPermisos.FirstOrDefaultAsync(rp =>
                        rp.RolId == rolId &&
                        rp.PermisoId == permission.Id);

                    if (existing == null)
                    {
                        context.RolPermisos.Add(new RolPermiso
                        {
                            RolId = rolId,
                            PermisoId = permission.Id,
                            IsGranted = true
                        });
                    }
                    else if (!existing.IsGranted)
                    {
                        existing.IsGranted = true;
                    }
                }
            }

            // Adminlos permisos
            await GrantPermissionsToRoleAsync(adminRole.Id, permissions);

            // Cajero: ventas, clientes y consulta de productos
            var cajeroPermissions = permissions
                 .Where(p => p.Name == "venta.ver" ||
                 p.Name == "venta.crear" ||
                 p.Name == "cliente.ver" ||
                 p.Name == "cliente.gestionar" ||
                 p.Name == "producto.ver" ||
                 p.Name == "devolucion.ver" ||
                 p.Name == "devolucion.crear")
             .ToList();
            await GrantPermissionsToRoleAsync(cajeroRole.Id, cajeroPermissions);

            // Gerente: supervisión y consulta operativa
            var gerentePermissions = permissions
                .Where(p => p.Name == "producto.ver" ||
                            p.Name == "categoria.ver" ||
                            p.Name == "marca.ver" ||
                            p.Name == "proveedor.ver" ||
                            p.Name == "cliente.ver" ||
                            p.Name == "empleado.ver" ||
                            p.Name == "cargo.ver" ||
                            p.Name == "almacen.ver" ||
                            p.Name == "sucursal.ver" ||
                            p.Name == "inventario.ver" ||
                            p.Name == "movimientoinventario.ver" ||
                            p.Name == "compra.ver" ||
                            p.Name == "venta.ver" ||
                            p.Name == "devolucion.ver" ||
                            p.Name == "metodopago.ver")
                .ToList();
            await GrantPermissionsToRoleAsync(gerenteRole.Id, gerentePermissions);

            // Bodeguero: compras, inventario, movimientos y consulta de productos
            var bodegueroPermissions = permissions
                .Where(p => p.Name == "compra.ver" ||
                            p.Name == "compra.crear" ||
                            p.Name == "inventario.ver" ||
                            p.Name == "inventario.actualizar" ||
                            p.Name == "movimientoinventario.ver" ||
                            p.Name == "movimientoinventario.crear" ||
                            p.Name == "producto.ver")
                .ToList();
            await GrantPermissionsToRoleAsync(bodegueroRole.Id, bodegueroPermissions);

            await context.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var cajeroRole = await context.Roles.FirstAsync(r => r.Name == "Cajero");
            var gerenteRole = await context.Roles.FirstAsync(r => r.Name == "Gerente");
            var bodegueroRole = await context.Roles.FirstAsync(r => r.Name == "Bodeguero");

            if (!await context.Usuarios.AnyAsync(u => u.UserName == "adminFerreteria"))
            {
                context.Usuarios.Add(new Usuario
                {
                    UserName = "adminFerreteria",
                    Email = "adminferreteria@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Admin123!"),
                    EmailConfirmed = true,
                    RolId = adminRole.Id
                });
            }

            if (!await context.Usuarios.AnyAsync(u => u.UserName == "cajero1"))
            {
                context.Usuarios.Add(new Usuario
                {
                    UserName = "cajero1",
                    Email = "cajeroferreteria@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Cajero123!"),
                    EmailConfirmed = true,
                    RolId = cajeroRole.Id
                });
            }

            if (!await context.Usuarios.AnyAsync(u => u.UserName == "gerente1"))
            {
                context.Usuarios.Add(new Usuario
                {
                    UserName = "gerente1",
                    Email = "gerenteferreteria@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Gerente123!"),
                    EmailConfirmed = true,
                    RolId = gerenteRole.Id
                });
            }

            if (!await context.Usuarios.AnyAsync(u => u.UserName == "bodeguero1"))
            {
                context.Usuarios.Add(new Usuario
                {
                    UserName = "bodeguero1",
                    Email = "bodegueroferreteria@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Bodeguero123!"),
                    EmailConfirmed = true,
                    RolId = bodegueroRole.Id
                });
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedMetodosPagosAsync(AppDbContext context)
        {
            if (await context.MetodoPagos.AnyAsync())
                return;

            context.MetodoPagos.AddRange(
                new MetodoPago { Name = "Efectivo" },
                new MetodoPago { Name = "Tarjeta de Credito/Debito" },
                new MetodoPago { Name = "Transferencia Bancaria" }
            );
            await context.SaveChangesAsync();
        }
    }
}