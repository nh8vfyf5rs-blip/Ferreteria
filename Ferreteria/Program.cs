using Ferreteria.Authorization;
using Ferreteria.Data;
using Ferreteria.Extensions;
using Ferreteria.Mapping;
using Ferreteria.Models.Setting;
using Ferreteria.Repositorios;
using Ferreteria.Service;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("Smtp"));

var jwtSettings =
    builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("No esta presente la configuracion del JWT");

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));

//======================= Repositorios =========================
builder.Services.AddScoped<IAlmacenRepository, AlmacenRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<IDevolucionesRepository, DevolucionesRepository>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
builder.Services.AddScoped<IDetalleInventarioRepository, DetalleInventarioRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<ISucursalRepository, SucursalRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();

//======================= Servicios ============================
builder.Services.AddScoped<IAlmacenService, AlmacenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IDevolucionesService, DevolucionesService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IDetalleInventarioService, DetalleInventarioService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IRolPermisoService, RolePermissionService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IVentaService, VentaService>();

//======================= DBContext ============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                context.HandleResponse();

                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/Login");
                }

                return Task.CompletedTask;
            },

            OnForbidden = context =>
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect("/Auth/AccessDenied");
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSingleton<IAuthorizationPolicyProvider, JwtPermiso.PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, JwtPermiso.PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin.panel", policy =>
        policy.RequireAssertion(context =>
            context.User.HasAnyPermission("rol.ver", "permiso.ver", "usuario.ver")));
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404 && !response.HasStarted)
    {
        response.Redirect("/Home/NotFoundPage");
    }

    await Task.CompletedTask;
});

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();