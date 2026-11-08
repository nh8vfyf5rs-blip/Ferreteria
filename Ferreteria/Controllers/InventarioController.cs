using Ferreteria.Authorization;
using Ferreteria.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ferreteria.Controllers
{
    [Authorize]
    public class InventarioController : Controller
    {
        private readonly AppDbContext _context;
        
    public InventarioController(AppDbContext context)
     {

            DbContext = context;

        }

        [TienePermiso("Inventario.Ver")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
