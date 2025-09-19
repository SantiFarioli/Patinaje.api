using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EvaluacionesController : ControllerBase
{
    private readonly AppPatinContext _db;
    public EvaluacionesController(AppPatinContext db) => _db = db;

    // DTO para crear
    public record CrearEvaluacionDto(
        int PatinadorId,
        string Elemento,
        DateTime Fecha,
        int Puntaje,             // 1..5
        string? Observaciones,
        string? VideoUrl
    );

    // POST /api/evaluaciones
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearEvaluacionDto dto)
    {
        if (dto.Puntaje < 1 || dto.Puntaje > 5)
            return BadRequest("Puntaje debe ser 1..5.");

        var existe = await _db.Patinadores.AnyAsync(p => p.PatinadorId == dto.PatinadorId);
        if (!existe) return BadRequest("PatinadorId no válido.");

        var e = new EvaluacionTecnica
        {
            PatinadorId = dto.PatinadorId,
            Elemento = dto.Elemento,
            Fecha = dto.Fecha,
            Puntaje = dto.Puntaje,
            Observaciones = dto.Observaciones,
            VideoUrl = dto.VideoUrl
        };

        _db.Evaluaciones.Add(e);
        await _db.SaveChangesAsync();

        // OJO: la PK es EvaluacionTecnicaId
        return CreatedAtAction(nameof(GetById), new { id = e.EvaluacionTecnicaId }, e);
    }

    // GET /api/evaluaciones?patinadorId=1&elemento=Axel&desde=2025-01-01&hasta=2025-12-31
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int? patinadorId,
        [FromQuery] string? elemento,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var q = _db.Evaluaciones.AsQueryable();

        if (patinadorId.HasValue) q = q.Where(e => e.PatinadorId == patinadorId.Value);
        if (!string.IsNullOrWhiteSpace(elemento)) q = q.Where(e => e.Elemento == elemento);
        if (desde.HasValue) q = q.Where(e => e.Fecha >= desde.Value);
        if (hasta.HasValue) q = q.Where(e => e.Fecha <= hasta.Value);

        var total = await q.CountAsync();
        var data = await q.OrderByDescending(e => e.Fecha)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();

        return Ok(new { total, page, pageSize, data });
    }

    // GET /api/evaluaciones/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Evaluaciones.FindAsync(id);
        return e is null ? NotFound() : Ok(e);
    }
}
