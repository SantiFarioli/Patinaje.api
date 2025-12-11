using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.DTOs.Dashboard;

namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppPatinContext _db;

        public DashboardController(AppPatinContext db)
        {
            _db = db;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalPatinadoras = await _db.Patinadores.CountAsync(p => p.Activo);
            var totalEventos = await _db.Torneos.CountAsync(t => t.FechaInicio >= DateTime.Today);
            var totalPagos = await _db.Pagos.CountAsync(p => p.Estado == "Pendiente");

            return Ok(new DashboardSummaryDto(totalPatinadoras, totalEventos, totalPagos));
        }

        [HttpGet("eventos")]
        public async Task<IActionResult> GetProximosEventos()
        {
            var eventos = await _db.Torneos
                .Where(t => t.FechaInicio >= DateTime.Today)
                .OrderBy(t => t.FechaInicio)
                .Take(5)
                .Select(t => new EventoDashboardDto(
                    t.TorneoId,
                    t.Nombre,
                    t.Lugar,
                    t.FechaInicio,
                    t.FechaFin,
                    t.Organizador
                ))
                .ToListAsync();

            return Ok(eventos);
        }
    }
}