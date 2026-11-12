using Ferreteria.Models;
using Ferreteria.Models.Segurudad;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolPermisos { get; set; }
        public DbSet<UsuarioPermiso> UsuariosPermisos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<MetodoPago> MetododoPagos { get; set; }
        public DbSet<Almacen>Almacenes { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<MovimientoInventario> MovimientoInventarios { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Devoluciones> Devoluciones { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }
        public DbSet<DetalleInventario> DetalleInventarios { get; set; }
        public DbSet<DetallePago> DetallePagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RolPermiso>().HasKey(rp => new { rp.RolId, rp.PermisoId });

            modelBuilder.Entity<RolPermiso>().
                HasOne(rp => rp.Rol)
                .WithMany(r => r.RolPermisos)
                .HasForeignKey(rp => rp.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolPermiso>()
              .HasOne(rp => rp.Permiso)
              .WithMany(p => p.RolPermissions)
              .HasForeignKey(rp => rp.PermisoId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
               .HasKey(up => new { up.UsuarioId, up.PermisoId });

            modelBuilder.Entity<UsuarioPermiso>()
             .HasOne(up => up.Usuario)
             .WithMany(u => u.UsuarioPermisos)
             .HasForeignKey(up => up.UsuarioId)
             .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioPermiso>()
           .HasOne(up => up.permiso)
           .WithMany(p => p.UsuarioPermisos)
           .HasForeignKey(up => up.PermisoId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Empleado>()
           .HasOne(e => e.Cargo)
           .WithMany(c => c.Empleados)
           .HasForeignKey(p => p.CargoId)
           .OnDelete(DeleteBehavior.Cascade);

          modelBuilder.Entity<Producto>()
         .HasOne(p => p.Categoria)
         .WithMany(c => c.Productos)
         .HasForeignKey(p => p.CategoriaId)
         .OnDelete(DeleteBehavior.Cascade);

          modelBuilder.Entity<Producto>()
         .HasOne(p => p.Marca)
         .WithMany(m => m.Productos)
         .HasForeignKey(p => p.MarcaId)
         .OnDelete(DeleteBehavior.Cascade);

        }

    }
}

