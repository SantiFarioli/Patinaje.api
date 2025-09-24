using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly AppPatinContext _db;
        public DashboardController(AppPatinContext db) => _db = db;

       
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var totalPatinadoras = await _db.Patinadores.CountAsync();

            var hoy = DateTime.Today;
            var a7  = hoy.AddDays(7);

            var eventosSemana = await _db.Torneos
                .CountAsync(t => t.FechaInicio <= a7 && t.FechaFin >= hoy);

            return Ok(new DashboardSummaryDto
            {
                TotalPatinadoras = totalPatinadoras,
                EventosSemana = eventosSemana
            });
        }

        // DTO anidado
        public class DashboardSummaryDto
        {
            public int TotalPatinadoras { get; set; }
            public int EventosSemana { get; set; }
        }
    }
}
