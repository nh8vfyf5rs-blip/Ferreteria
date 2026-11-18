using System.Security.Claims;
using Ferreteria.DTOS;
using Ferreteria.Extensions;
using Ferreteria.Helpers;
using Ferreteria.Repositorios;
using Ferreteria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ferreteria.Controllers
{
    public class AdminController(
        IUsuarioRepository usuarioRepository,
        IRolePermissionService rolePermissionService,
        IPermisoService permisoService,
        IRolService rolService,
        IUsuarioService usuarioService) : Controller
    {
        private async Task<IReadOnlyList<string>> GetEffectivePermissionAsync()
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId))
            {
                return User.GetPermission();
            }

            var dbUser = await usuarioRepository
                .GetUsuarioByIdWhitPermissionAsync(userId);

            return dbUser != null
                ? EffectivePermission.FromUser(dbUser)
                : User.GetPermission();
        }

        // =========================================================
        // PANEL DE ADMINISTRACIÓN
        // =========================================================

        [Authorize(Policy = "admin.panel")]
        public IActionResult Index()
        {
            ViewBag.Permissions = User.GetPermission();

            return View();
        }

        // =========================================================
        // ADMINISTRACIÓN DE PERMISOS
        // =========================================================

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionData()
        {
            var permissions = await permisoService.GetAllAsync();

            return Ok(permissions);
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> Permissions()
        {
            var effective = await GetEffectivePermissionAsync();

            ViewBag.CanCreate = effective.Contains(
                "permiso.crear",
                StringComparer.OrdinalIgnoreCase);

            ViewBag.CanUpdate = effective.Any(p =>
                string.Equals(
                    p,
                    "permiso.actualizar",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    p,
                    "permiso.crear",
                    StringComparison.OrdinalIgnoreCase));

            ViewBag.CanDelete = effective.Any(p =>
                string.Equals(
                    p,
                    "permiso.borrar",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    p,
                    "permiso.crear",
                    StringComparison.OrdinalIgnoreCase));

            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await permisoService.GetByIdAsync(id);

            if (permission == null)
                return NotFound(new { message = "Permiso no encontrado" });

            return Ok(permission);
        }

        [Authorize(Policy = "permiso.crear")]
        [HttpPost]
        public async Task<IActionResult> CreatePermission(
            [FromBody] CreatePermisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Revise los datos del formulario"
                });
            }

            var (success, errorMessage) =
                await permisoService.CreateAsync(dto);

            if (!success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = errorMessage
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Permiso '{dto.Name}' creado correctamente"
            });
        }

        [Authorize(Policy = "permiso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> UpdatePermission(
            int id,
            [FromBody] UpdatePermisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Revise los datos del formulario"
                });
            }

            var (success, errorMessage) =
                await permisoService.UpdateAsync(id, dto);

            if (!success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = errorMessage
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Permiso '{dto.Name}' actualizado correctamente"
            });
        }

        [Authorize(Policy = "permiso.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var (success, errorMessage) =
                await permisoService.DeleteAsync(id);

            if (!success)
            {
                return BadRequest(new
                {
                    success = false,
                    message = errorMessage
                });
            }

            return Ok(new
            {
                success = true,
                message = "Permiso eliminado correctamente"
            });
        }

        // =========================================================
        // ADMINISTRACIÓN DE ROLES
        // =========================================================

        [Authorize(Policy = "rol.ver")]
        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            var roles = await rolService.GetAllAsync();

            return View(roles);
        }

        [Authorize(Policy = "rol.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRol(CreateRolDto dto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await rolService.GetAllAsync();

                ViewBag.CreateError = "El nombre del rol es obligatorio";

                return View("Roles", roles);
            }

            var (success, errorMessage) =
                await rolService.CreateAsync(dto);

            if (!success)
            {
                var roles = await rolService.GetAllAsync();

                ViewBag.CreateError = errorMessage;

                return View("Roles", roles);
            }

            TempData["Success"] =
                $"Rol '{dto.Name}' creado correctamente";

            return RedirectToAction(nameof(Roles));
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpGet]
        public async Task<IActionResult> RoleDetails(int id)
        {
            var role = await rolService.GetByIdAsync(id);

            if (role == null)
                return NotFound();

            var allPermissions =
                await rolePermissionService.GetAllPermissionsAsync();

            var rolePermissions =
                await rolePermissionService.GetRolePermissionsAsync(id);

            ViewBag.AllPermission = allPermissions;
            ViewBag.RolePermissions = rolePermissions;

            return View(role);
        }

        [Authorize(Policy = "rol.actualizar")]
        [HttpPost]
        public async Task<IActionResult> UpdateRol(
            int id,
            [FromBody] UpdateRolDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Revise los datos del formulario"
                });
            }

            var (success, errorMessage) =
                await rolService.UpdateAsync(id, dto);

            return Json(new
            {
                success,
                message = success
                    ? "Rol actualizado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "rol.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var (success, errorMessage) =
                await rolService.DeleteAsync(id);

            return Json(new
            {
                success,
                message = success
                    ? "Rol eliminado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(
            int roleId,
            int permissionId)
        {
            var (success, errorMessage) =
                await rolePermissionService.AssignPermissionToRoleAsync(
                    roleId,
                    permissionId);

            return Json(new
            {
                success,
                message = success
                    ? "Permiso asignado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemovePermissionFromRole(
            int roleId,
            int permissionId)
        {
            var (success, errorMessage) =
                await rolePermissionService.RemovePermissionFromRoleAsync(
                    roleId,
                    permissionId);

            return Json(new
            {
                success,
                message = success
                    ? "Permiso removido correctamente"
                    : errorMessage
            });
        }

        // =========================================================
        // ADMINISTRACIÓN DE USUARIOS
        // =========================================================

        [Authorize(Policy = "usuario.ver")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await usuarioService.GetAllAsync();

            return View(users);
        }

        [Authorize(Policy = "usuario.ver")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await usuarioService.GetByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(user);
        }

        [Authorize(Policy = "usuario.crear")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(
            [FromBody] CreateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Revise los datos del formulario"
                });
            }

            var (success, errorMessage) =
                await usuarioService.CreateAsync(dto);

            return Json(new
            {
                success,
                message = success
                    ? "Usuario creado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.actualizar")]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(
            int id,
            [FromBody] UpdateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Revise los datos del formulario"
                });
            }

            var (success, errorMessage) =
                await usuarioService.UpdateAsync(id, dto);

            return Json(new
            {
                success,
                message = success
                    ? "Usuario actualizado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var (success, errorMessage) =
                await usuarioService.DeleteAsync(id);

            return Json(new
            {
                success,
                message = success
                    ? "Usuario eliminado correctamente"
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpGet]
        public async Task<IActionResult> UserPermissions(int id)
        {
            var user =
                await rolePermissionService.GetUserPermissionsAsync(id);

            if (user == null)
                return NotFound();

            var allPermissions =
                await rolePermissionService.GetAllPermissionsAsync();

            var allRoles =
                await rolePermissionService.GetAllRolesAsync();

            ViewBag.AllPermissions = allPermissions;
            ViewBag.AllRoles = allRoles;
            ViewBag.CanChangeRole =
                User.HasPermission("usuario.cambiar_rol");

            return View(user);
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToUser(
            int userId,
            [FromBody] AssingUserPermisoDto dto)
        {
            var (success, errorMessage) =
                await rolePermissionService.AssignPermissionToUserAsync(
                    userId,
                    dto);

            return Json(new
            {
                success,
                message = success
                    ? (dto.IsGranted
                        ? "Permiso concedido al usuario"
                        : "Permiso denegado al usuario")
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserPermission(
            int userId,
            int permissionId)
        {
            var (success, errorMessage) =
                await rolePermissionService.RemoveUserPermissionAsync(
                    userId,
                    permissionId);

            return Json(new
            {
                success,
                message = success
                    ? "Asignación directa removida"
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.cambiar_rol")]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(
            int userId,
            int roleId)
        {
            var (success, errorMessage) =
                await rolePermissionService.UpdateUserRoleAsync(
                    userId,
                    roleId);

            return Json(new 
            {
                success,
                message = success
                    ? "Rol actualizado correctamente"
                    : errorMessage
            });
        }
    }
}
