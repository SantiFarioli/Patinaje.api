using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InscripcionesController : ControllerBase
{
    private readonly AppPatinContext _db;
    public InscripcionesController(AppPatinContext db) => _db = db;

    public record InscripcionCreateDto(
        int TorneoId,
        int PatinadorId,
        string CategoriaCompetencia,
        decimal CostoInscripcion,
        string EstadoPago,    // "Pendiente" | "Pagado"
        string? Resultado
    );

    public record InscripcionUpdateDto(
        string CategoriaCompetencia,
        decimal CostoInscripcion,
        string EstadoPago,
        string? Resultado
    );

    // GET /api/inscripciones?torneoId=1&patinadorId=2
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? torneoId, [FromQuery] int? patinadorId)
    {
        var q = _db.Inscripciones.AsQueryable();
        if (torneoId.HasValue) q = q.Where(i => i.TorneoId == torneoId.Value);
        if (patinadorId.HasValue) q = q.Where(i => i.PatinadorId == patinadorId.Value);

        var data = await q
            .OrderByDescending(i => i.InscripcionTorneoId)
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var i = await _db.Inscripciones.FindAsync(id);
        return i is null ? NotFound() : Ok(i);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InscripcionCreateDto dto)
    {
        var torneoOk = await _db.Torneos.AnyAsync(t => t.TorneoId == dto.TorneoId);
        if (!torneoOk) return BadRequest("TorneoId no válido.");

        var patOk = await _db.Patinadores.AnyAsync(p => p.PatinadorId == dto.PatinadorId);
        if (!patOk) return BadRequest("PatinadorId no válido.");

        var entity = new InscripcionTorneo
        {
            TorneoId = dto.TorneoId,
            PatinadorId = dto.PatinadorId,
            CategoriaCompetencia = dto.CategoriaCompetencia,
            CostoInscripcion = dto.CostoInscripcion,
            EstadoPago = dto.EstadoPago,
            Resultado = dto.Resultado
        };

        _db.Inscripciones.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.InscripcionTorneoId }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] InscripcionUpdateDto dto)
    {
        var entity = await _db.Inscripciones.FindAsync(id);
        if (entity is null) return NotFound();

        entity.CategoriaCompetencia = dto.CategoriaCompetencia;
        entity.CostoInscripcion = dto.CostoInscripcion;
        entity.EstadoPago = dto.EstadoPago;
        entity.Resultado = dto.Resultado;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var i = await _db.Inscripciones.FindAsync(id);
        if (i is null) return NotFound();
        _db.Remove(i);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
