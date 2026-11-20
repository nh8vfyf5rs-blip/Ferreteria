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
                var changed = false;

                if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
                {
                    context.Roles.Add(new Rol { Name = "Admin" });
                    changed = true;
                }

                if (!await context.Roles.AnyAsync(r => r.Name == "Cajero"))
                {
                    context.Roles.Add(new Rol { Name = "Cajero" });
                    changed = true;
                }

                if (!await context.Roles.AnyAsync(r => r.Name == "Bodeguero"))
                {
                    context.Roles.Add(new Rol { Name = "Bodeguero" });
                    changed = true;
                }
                if (changed)
                    await context.SaveChangesAsync();
            }

        private static async Task SeedPermissionsAsync(AppDbContext context)
        {
            if (await context.Permisos.AnyAsync())
                return;

            context.Permisos.AddRange(
                new Permiso { Name = "admin.panel", Descripcion = "Acceso al panel administrativo" },

                new Permiso { Name = "permiso.ver", Descripcion = "Ver permisos" },
                new Permiso { Name = "permiso.crear", Descripcion = "Crear permiso" },
                new Permiso { Name = "permiso.actualizar", Descripcion = "Actualizar permiso" },
                new Permiso { Name = "permiso.borrar", Descripcion = "Borrar permiso" },

                new Permiso { Name = "rol.ver", Descripcion = "Ver roles del sistema" },
                new Permiso { Name = "rol.crear", Descripcion = "Crear rol" },
                new Permiso { Name = "rol.asignar", Descripcion = "Asignar permisos a roles" },

                new Permiso { Name = "usuario.ver", Descripcion = "Ver usuarios del sistema" },
                new Permiso { Name = "usuario.asignar", Descripcion = "Asignar roles y permisos a usuarios" },
                new Permiso { Name = "usuario.cambiar_rol", Descripcion = "Cambiar rol de usuario" },

                new Permiso { Name = "producto.ver", Descripcion = "Ver listado de productos" },
                new Permiso { Name = "producto.crear", Descripcion = "Crear nuevo producto" },
                new Permiso { Name = "producto.actualizar", Descripcion = "Actualizar producto" },
                new Permiso { Name = "producto.borrar", Descripcion = "Eliminar producto" },

                new Permiso { Name = "categoria.ver", Descripcion = "Ver categorías" },
                new Permiso { Name = "categoria.crear", Descripcion = "Crear categoría" },
                new Permiso { Name = "categoria.actualizar", Descripcion = "Actualizar categoría" },
                new Permiso { Name = "categoria.borrar", Descripcion = "Eliminar categoría" },

                new Permiso { Name = "marca.ver", Descripcion = "Ver marcas" },
                new Permiso { Name = "marca.crear", Descripcion = "Crear marca" },
                new Permiso { Name = "marca.actualizar", Descripcion = "Actualizar marca" },
                new Permiso { Name = "marca.borrar", Descripcion = "Eliminar marca" },

                new Permiso { Name = "proveedor.ver", Descripcion = "Ver proveedores" },
                new Permiso { Name = "proveedor.crear", Descripcion = "Crear proveedor" },
                new Permiso { Name = "proveedor.actualizar", Descripcion = "Actualizar proveedor" },
                new Permiso { Name = "proveedor.borrar", Descripcion = "Eliminar proveedor" },

                new Permiso { Name = "cliente.ver", Descripcion = "Ver clientes" },
                new Permiso { Name = "cliente.crear", Descripcion = "Crear cliente" },
                new Permiso { Name = "cliente.actualizar", Descripcion = "Actualizar cliente" },
                new Permiso { Name = "cliente.borrar", Descripcion = "Eliminar cliente" },
                new Permiso { Name = "cliente.gestionar", Descripcion = "Gestionar clientes" },

                new Permiso { Name = "empleado.ver", Descripcion = "Ver empleados" },
                new Permiso { Name = "empleado.crear", Descripcion = "Crear empleado" },
                new Permiso { Name = "empleado.actualizar", Descripcion = "Actualizar empleado" },
                new Permiso { Name = "empleado.borrar", Descripcion = "Eliminar empleado" },

                new Permiso { Name = "cargo.ver", Descripcion = "Ver cargos" },
                new Permiso { Name = "cargo.crear", Descripcion = "Crear cargo" },
                new Permiso { Name = "cargo.actualizar", Descripcion = "Actualizar cargo" },
                new Permiso { Name = "cargo.borrar", Descripcion = "Eliminar cargo" },

                new Permiso { Name = "almacen.ver", Descripcion = "Ver almacenes" },
                new Permiso { Name = "almacen.crear", Descripcion = "Crear almacén" },
                new Permiso { Name = "almacen.actualizar", Descripcion = "Actualizar almacén" },
                new Permiso { Name = "almacen.borrar", Descripcion = "Eliminar almacén" },

                new Permiso { Name = "sucursal.ver", Descripcion = "Ver sucursales" },
                new Permiso { Name = "sucursal.crear", Descripcion = "Crear sucursal" },
                new Permiso { Name = "sucursal.actualizar", Descripcion = "Actualizar sucursal" },
                new Permiso { Name = "sucursal.borrar", Descripcion = "Eliminar sucursal" },

                new Permiso { Name = "inventario.ver", Descripcion = "Ver inventario" },
                new Permiso { Name = "inventario.crear", Descripcion = "Crear inventario" },
                new Permiso { Name = "inventario.actualizar", Descripcion = "Actualizar stock" },
                new Permiso { Name = "inventario.borrar", Descripcion = "Eliminar inventario" },

                new Permiso { Name = "movimientoinventario.ver", Descripcion = "Ver movimientos de inventario" },
                new Permiso { Name = "movimientoinventario.crear", Descripcion = "Crear movimiento de inventario" },
                new Permiso { Name = "movimientoinventario.actualizar", Descripcion = "Actualizar movimiento de inventario" },
                new Permiso { Name = "movimientoinventario.borrar", Descripcion = "Eliminar movimiento de inventario" },

                new Permiso { Name = "compra.ver", Descripcion = "Ver compras a proveedores" },
                new Permiso { Name = "compra.crear", Descripcion = "Registrar compras de mercadería" },
                new Permiso { Name = "compra.actualizar", Descripcion = "Actualizar compra" },
                new Permiso { Name = "compra.borrar", Descripcion = "Eliminar compra" },

                new Permiso { Name = "venta.ver", Descripcion = "Ver ventas realizadas" },
                new Permiso { Name = "venta.crear", Descripcion = "Registrar nueva venta" },
                new Permiso { Name = "venta.actualizar", Descripcion = "Actualizar venta" },
                new Permiso { Name = "venta.borrar", Descripcion = "Eliminar venta" },

                new Permiso { Name = "devolucion.ver", Descripcion = "Ver devoluciones" },
                new Permiso { Name = "devolucion.crear", Descripcion = "Crear devolución" },
                new Permiso { Name = "devolucion.actualizar", Descripcion = "Actualizar devolución" },
                new Permiso { Name = "devolucion.borrar", Descripcion = "Eliminar devolución" },

                new Permiso { Name = "metodopago.ver", Descripcion = "Ver métodos de pago" },
                new Permiso { Name = "metodopago.crear", Descripcion = "Crear método de pago" },
                new Permiso { Name = "metodopago.actualizar", Descripcion = "Actualizar método de pago" },
                new Permiso { Name = "metodopago.borrar", Descripcion = "Eliminar método de pago" }
            );

            await context.SaveChangesAsync();
        }


        private static async Task SeedRolePermissionsAsync(AppDbContext context)
        
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                var cajeroRole = await context.Roles.FirstAsync(r => r.Name == "Cajero");
                var bodegueroRole = await context.Roles.FirstAsync(r => r.Name == "Bodeguero");

                var permissions = await context.Permisos.ToListAsync();

                foreach (var permission in permissions)
                {
                    if (!await context.RolPermisos.AnyAsync(rp =>
                        rp.RolId == adminRole.Id &&
                        rp.PermisoId == permission.Id))
                    {
                        context.RolPermisos.Add(new RolPermiso
                        {
                            RolId = adminRole.Id,
                            PermisoId = permission.Id,
                            IsGranted = true
                        });
                    }
                }

                var cajeroPermissions = permissions
                    .Where(p => p.Name == "venta.ver" ||
                                p.Name == "venta.crear" ||
                                 p.Name == "cliente.ver" ||
                                p.Name == "cliente.gestionar" ||
                                p.Name == "producto.ver") 

                    .ToList();

                foreach (var permission in cajeroPermissions)
                {
                    if (!await context.RolPermisos.AnyAsync(rp =>
                        rp.RolId == cajeroRole.Id &&
                        rp.PermisoId == permission.Id))
                    {
                        context.RolPermisos.Add(new RolPermiso
                        {
                            RolId = cajeroRole.Id,
                            PermisoId = permission.Id,
                            IsGranted = true
                        });
                    }
                }

                var bodegueroPermissions = permissions
                    .Where(p => p.Name == "compra.ver" ||
                                p.Name == "compra.crear" ||
                                p.Name == "inventario.ver" ||
                                p.Name == "inventario.actualizar" ||
                                p.Name == "producto.ver")
                    .ToList();

                foreach (var permission in bodegueroPermissions)
                {
                    if (!await context.RolPermisos.AnyAsync(rp =>
                        rp.RolId == bodegueroRole.Id &&
                        rp.PermisoId == permission.Id))
                    {
                        context.RolPermisos.Add(new RolPermiso
                        {
                            RolId = bodegueroRole.Id,
                            PermisoId = permission.Id,
                            IsGranted = true
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        

        private static async Task SeedUsersAsync(AppDbContext context)
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            var cajeroRole = await context.Roles.FirstAsync(r => r.Name == "Cajero");
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
                new MetodoPago { Name = "Transferencia Bancaria"}
            );
            await context.SaveChangesAsync();
        }
    }
}