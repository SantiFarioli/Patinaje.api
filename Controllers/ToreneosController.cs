using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TorneosController : ControllerBase
{
    private readonly AppPatinContext _db;
    public TorneosController(AppPatinContext db) => _db = db;

    public record TorneoCreateDto(
        string Nombre,
        string Lugar,
        DateTime FechaInicio,
        DateTime FechaFin,
        DateTime FechaLimiteInscripcion,
        string Organizador
    );

    public record TorneoUpdateDto(
        string Nombre,
        string Lugar,
        DateTime FechaInicio,
        DateTime FechaFin,
        DateTime FechaLimiteInscripcion,
        string Organizador
    );

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool? proximos)
    {
        var q = _db.Torneos.AsQueryable();
        if (proximos == true)
            q = q.Where(t => t.FechaInicio >= DateTime.Today);

        var data = await q.OrderBy(t => t.FechaInicio).ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Torneos.FindAsync(id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TorneoCreateDto dto)
    {
        var t = new Torneo
        {
            Nombre = dto.Nombre,
            Lugar = dto.Lugar,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            FechaLimiteInscripcion = dto.FechaLimiteInscripcion,
            Organizador = dto.Organizador
        };
        _db.Torneos.Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = t.TorneoId }, t);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TorneoUpdateDto dto)
    {
        var t = await _db.Torneos.FindAsync(id);
        if (t is null) return NotFound();

        t.Nombre = dto.Nombre;
        t.Lugar = dto.Lugar;
        t.FechaInicio = dto.FechaInicio;
        t.FechaFin = dto.FechaFin;
        t.FechaLimiteInscripcion = dto.FechaLimiteInscripcion;
        t.Organizador = dto.Organizador;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Torneos.FindAsync(id);
        if (t is null) return NotFound();
        _db.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
