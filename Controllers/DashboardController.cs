using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppPatinContext _db;
    private readonly IMemoryCache _cache;

    public DashboardController(AppPatinContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // DTOs
    public record DashboardSummaryDto(
        int TotalPatinadoras,
        int TotalEventosProximos,
        int TotalPagosPendientes
    );

    public record EventoDto(
        int TorneoId,
        string Nombre,
        string Lugar,
        DateTime FechaInicio,
        DateTime FechaFin,
        DateTime FechaLimiteInscripcion,
        string Organizador
    );

    // GET /api/dashboard/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        const string cacheKey = "dashboard_summary";

        if (!_cache.TryGetValue(cacheKey, out DashboardSummaryDto? summary))
        {
            var totalPatinadoras = await _db.Patinadores.CountAsync(p => p.Activo);
            var totalEventosProximos = await _db.Torneos.CountAsync(t => t.FechaInicio >= DateTime.Today);
            var totalPagosPendientes = await _db.Pagos.CountAsync(p => p.Estado == "Pendiente");

            summary = new DashboardSummaryDto(
                totalPatinadoras,
                totalEventosProximos,
                totalPagosPendientes
            );

            // Cache por 60s
            _cache.Set(cacheKey, summary, TimeSpan.FromSeconds(60));
        }

        return Ok(summary);
    }

    // GET /api/dashboard/eventos
    [HttpGet("eventos")]
    public async Task<IActionResult> GetEventos()
    {
        const string cacheKey = "dashboard_eventos";

        if (!_cache.TryGetValue(cacheKey, out List<EventoDto>? eventos))
        {
            eventos = await _db.Torneos
                .Where(t => t.FechaFin >= DateTime.Today) // solo los que no terminaron
                .OrderBy(t => t.FechaInicio)
                .Select(t => new EventoDto(
                    t.TorneoId,
                    t.Nombre,
                    t.Lugar,
                    t.FechaInicio,
                    t.FechaFin,
                    t.FechaLimiteInscripcion,
                    t.Organizador
                ))
                .ToListAsync();

            _cache.Set(cacheKey, eventos, TimeSpan.FromSeconds(60));
        }

        return Ok(eventos);
    }
}
