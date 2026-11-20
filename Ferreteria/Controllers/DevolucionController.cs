using Microsoft.AspNetCore.Mvc;

namespace FierroNic.Controllers
{
    public class DevolucionesController : Controller
    {
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult GetData()
        {
            // Reemplaza esto con tu consulta de Entity Framework Core
            var lista = new[]
            {
                new { id = 1, fecha = DateTime.Today, ventaReferencia = "V-0001", motivo = "Producto dañado", total = 150.00m }
            };
            return Json(new { data = lista });
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            return Json(new { success = true, data = new { id, fecha = DateTime.Today, ventaId = 1, total = 150.00m, motivo = "Ejemplo" } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromForm] DevolucionInputModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos." });

            return Json(new { success = true, message = "Devolución creada correctamente." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, [FromForm] DevolucionInputModel model)
        {
            if (id <= 0 || !ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos para actualizar." });

            return Json(new { success = true, message = "Devolución actualizada correctamente." });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return Json(new { success = false, message = "ID inválido." });

            return Json(new { success = true, message = "Devolución eliminada correctamente." });
        }
    }

    public class DevolucionInputModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int VentaId { get; set; }
        public decimal Total { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }
}
