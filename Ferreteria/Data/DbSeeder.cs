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
                new Categoria { Name = "Fonteria" },
                new Categoria { Name = "Electricidad" }

            );
        }

        private static async Task SeedRolesAsync(AppDbContext context)
        {
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
        }
    private static async Task SeedPermissionsAsync(AppDbContext context)
        { 
                if (await context.Permisos.AnyAsync())
                    return;

                context.Permisos.AddRange(
                    new Permiso { Name = "producto.ver", Descripcion = "Ver listado de productos" },
                    new Permiso { Name = "producto.crear", Descripcion = "Crear nuevo producto" },
                    new Permiso { Name = "producto.actualizar", Descripcion = "Actualizar producto" },
                    new Permiso { Name = "producto.borrar", Descripcion = "Eliminar producto" },

                    new Permiso { Name = "Venta.ver", Descripcion = "Ver Ventas realizadas" },
                    new Permiso { Name = "Venta.crear", Descripcion = "Registrar nueva Venta" },
                    new Permiso { Name = "Venta.asignar", Descripcion = "Gestionar clientes de la ferreteria" },

                    new Permiso { Name = "compra.ver", Descripcion = "Ver compras a proveedores" },
                    new Permiso { Name = "compra.crear", Descripcion = "Registrar compras de mercaderia" },
                    new Permiso { Name = "Inventario.actualizar", Descripcion = "Ver stock en sucursaleso" },
                    new Permiso { Name = "permiso.borrar", Descripcion = "Borrar permiso" },

                    new Permiso { Name = "rol.ver", Descripcion = "Ver roles del sistema" },
                    new Permiso { Name = "rol.asignar", Descripcion = "Asignar permisos a roles" },
                    new Permiso { Name = "usuario.ver", Descripcion = "ver  usuario del sistema" },
                    new Permiso { Name = "usuario.asignar", Descripcion = "Asignar roles y permisos a usuarios" }
                );

                await context.SaveChangesAsync();         
        }


        private static async Task SeedRolePermissionsAsync(AppDbContext context)
        {
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

            if (await context.MetododoPagos.AnyAsync())
                return;


            context.MetododoPagos.AddRange(
                new MetodoPago { Name = "Efectivo" },
                new MetodoPago { Name = "Tarjeta de Credito/Debito" },
                new MetodoPago { Name = "Transferencia Bancaria"}
            );
            await context.SaveChangesAsync();
        }
    }
}