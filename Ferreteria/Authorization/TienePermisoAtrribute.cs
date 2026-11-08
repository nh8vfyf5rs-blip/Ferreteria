using Ferreteria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ferreteria.Authorization
{
    public class TienePermisoAtrribute : TypeFilterAttribute
    {
        public TienePermisoAtrribute(String nombrePermiso) : base(typeof(PermisoFilter)
        {
            Arguments = new object[] { nombrePermiso };
        }
        private class PermisoFilter : IAsyncActionFilter
        {
            private readonly string _nombrePermiso;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool autorizado = true;
            if (!autorizado)
            {
                context.Result = new RedirectToActionResult("AccesoDenegado", "Auth", null);
            }
            await next();
        }
    }
}
