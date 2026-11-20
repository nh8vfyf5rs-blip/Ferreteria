using Ferreteria.DTOS;
using Ferreteria.Extensions;
using Ferreteria.Helpers;
using Ferreteria.Repositorios;
using Ferreteria.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ferreteria.Controllers
{
    public class AdminController(
        IUsuarioRepository _userRepository,
        IRolPermisoService _rolPermisoService) : Controller
    {
        private async Task<IReadOnlyList<string>> GetEffectivePermissionAsync()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return User.GetPermission();

            var dbUser = await _userRepository.GetUsuarioByIdWhitPermissionAsync(userId);
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
        [HttpPost]
        public async Task<IActionResult> GetPermissionsData([FromForm] DataTablesRequestDto request)
        {
            var result = await _rolPermisoService.GetPermissionsPagedAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> Permissions()
        {
            var effective = await GetEffectivePermissionAsync();

            ViewBag.CanCreate = effective.Contains("permiso.crear", StringComparer.OrdinalIgnoreCase);
            ViewBag.CanUpdate = effective.Any(p =>
                string.Equals(p, "permiso.actualizar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));
            ViewBag.CanDelete = effective.Any(p =>
                string.Equals(p, "permiso.borrar", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p, "permiso.crear", StringComparison.OrdinalIgnoreCase));

            return View();
        }

        [Authorize(Policy = "permiso.ver")]
        [HttpGet]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            var permission = await _rolPermisoService.GetPermissionByIdAsync(id);
            if (permission == null)
                return NotFound(new { message = "Permiso no encontrado" });

            return Ok(permission);
        }

        [Authorize(Policy = "permiso.crear")]
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] SavePermisoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Revise los datos del formulario" });

            var (success, _, errorMessage) = await _rolPermisoService.CreatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = $"Permiso '{dto.Name}' creado correctamente" });
        }

        [Authorize(Policy = "permiso.actualizar")]
        [HttpPost]
        public async Task<IActionResult> UpdatePermission([FromBody] SavePermisoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Revise los datos del formulario" });

            var (success, errorMessage) = await _rolPermisoService.UpdatePermissionAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = $"Permiso '{dto.Name}' actualizado correctamente" });
        }

        [Authorize(Policy = "permiso.borrar")]
        [HttpPost]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var (success, errorMessage) = await _rolPermisoService.DeletePermissionAsync(id);
            if (!success)
                return BadRequest(new { success = false, message = errorMessage });

            return Ok(new { success = true, message = "Permiso eliminado correctamente" });
        }

        // =========================================================
        // ADMINISTRACIÓN DE ROLES
        // =========================================================

        [Authorize(Policy = "rol.ver")]
        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            var roles = await _rolPermisoService.GetAllRolesAsync();
            return View(roles);
        }

        [Authorize(Policy = "rol.crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateRolDto dto)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _rolPermisoService.GetAllRolesAsync();
                ViewBag.CreateError = "El nombre del rol es obligatorio";
                return View("Roles", roles);
            }

            var (success, _, errorMessage) = await _rolPermisoService.CreateRoleAsync(dto);
            if (!success)
            {
                var roles = await _rolPermisoService.GetAllRolesAsync();
                ViewBag.CreateError = errorMessage;
                return View("Roles", roles);
            }

            TempData["Success"] = $"Rol '{dto.Name}' creado correctamente";
            return RedirectToAction(nameof(Roles));
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpGet]
        public async Task<IActionResult> RoleDetails(int id)
        {
            var role = await _rolPermisoService.GetRoleByIdAsync(id);
            if (role == null)
                return NotFound();

            var allPermissions = await _rolPermisoService.GetAllPermissionsAsync();
            ViewBag.AllPermission = allPermissions;

            return View(role);
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolPermisoService.AssignPermissionToRoleAsync(roleId, permissionId);
            return Json(new { success, message = success ? "Permiso asignado correctamente" : errorMessage });
        }

        [Authorize(Policy = "rol.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemovePermissionFromRole(int roleId, int permissionId)
        {
            var (success, errorMessage) = await _rolPermisoService.RemovePermissionFromRoleAsync(roleId, permissionId);
            return Json(new { success, message = success ? "Permiso removido correctamente" : errorMessage });
        }

        // =========================================================
        // ADMINISTRACIÓN DE USUARIOS
        // =========================================================

        [Authorize(Policy = "usuario.ver")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _rolPermisoService.GetAllUsersAsync();
            return View(users);
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpGet]
        public async Task<IActionResult> UserPermissions(int id)
        {
            var user = await _rolPermisoService.GetUserPermissionsAsync(id);
            if (user == null)
                return NotFound();

            var allPermissions = await _rolPermisoService.GetAllPermissionsAsync();
            var allRoles = await _rolPermisoService.GetAllRolesAsync();

            ViewBag.AllPermissions = allPermissions;
            ViewBag.AllRoles = allRoles;
            ViewBag.CanChangeRole = User.HasPermission("usuario.cambiar_rol");

            return View(user);
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> AssignPermissionToUser(int userId, [FromBody] AssingUserPermisoDto dto)
        {
            var (success, errorMessage) = await _rolPermisoService.AssignPermissionToUserAsync(userId, dto);
            return Json(new
            {
                success,
                message = success
                    ? (dto.IsGranted ? "Permiso concedido al usuario." : "Permiso denegado al usuario.")
                    : errorMessage
            });
        }

        [Authorize(Policy = "usuario.asignar")]
        [HttpPost]
        public async Task<IActionResult> RemoveUserPermission(int userId, int permissionId)
        {
            var (success, errorMessage) = await _rolPermisoService.RemoveUserPermissionAsync(userId, permissionId);
            return Json(new { success, message = success ? "Asignación directa removida" : errorMessage });
        }

        [Authorize(Policy = "usuario.cambiar_rol")]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] UpdateUserRoleDto dto)
        {
            var (success, errorMessage) = await _rolPermisoService.UpdateUserRoleAsync(userId, dto.RolId);
            return Json(new { success, message = success ? "Rol actualizado correctamente" : errorMessage });
        }
    }
}