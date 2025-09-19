using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciasController : ControllerBase
{
    private readonly AppPatinContext _db;
    public AsistenciasController(AppPatinContext db) => _db = db;

    public record TomarAsistenciaItem(int PatinadorId, bool Presente);
    public record TomarAsistenciaDto(DateTime Fecha, int? ClaseId, List<TomarAsistenciaItem> Items);

    // POST /api/asistencias  (toma de lista en batch)
    [HttpPost]
    public async Task<IActionResult> Tomar([FromBody] TomarAsistenciaDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest("Debes enviar al menos un item.");

        // opcional: limpiar registros previos del mismo día/clase y patinadores
        var pIds = dto.Items.Select(i => i.PatinadorId).ToList();
        var prev = await _db.Asistencias
            .Where(a => a.FechaClase.Date == dto.Fecha.Date && pIds.Contains(a.PatinadorId))
            .ToListAsync();
        _db.Asistencias.RemoveRange(prev);

        var nuevas = dto.Items.Select(i => new Asistencia
        {
            PatinadorId = i.PatinadorId,
            FechaClase = dto.Fecha,
            Presente = i.Presente
        });
        await _db.Asistencias.AddRangeAsync(nuevas);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/asistencias?fecha=2025-10-10&patinadorId=1
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? fecha,
        [FromQuery] int? patinadorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var q = _db.Asistencias.AsQueryable();

        if (fecha.HasValue) q = q.Where(a => a.FechaClase.Date == fecha.Value.Date);
        if (patinadorId.HasValue) q = q.Where(a => a.PatinadorId == patinadorId.Value);

        var total = await q.CountAsync();
        var data = await q.OrderByDescending(a => a.FechaClase)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();

        return Ok(new { total, page, pageSize, data });
    }
}
