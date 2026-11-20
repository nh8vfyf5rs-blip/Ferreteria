using Ferreteria.Extensions;
using Ferreteria.Helpers;
using Ferreteria.Repositorios;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ferreteria.ViewComponets
{
    public class NavigationViewComponent(IUsuarioRepository _userRepository) : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.User as ClaimsPrincipal;
            var permiso = user?.GetPermission() ?? [];
            if (user?.Identity?.IsAuthenticated == true &&
                int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
            {
                var dbUser = await _userRepository.GetUsuarioByIdWhitPermissionAsync(id);
                if (dbUser != null)
                    permiso = EffectivePermission.FromUser(dbUser);
            }
            return View(permiso);
        }
    }
}
