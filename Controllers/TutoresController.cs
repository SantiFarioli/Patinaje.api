using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TutoresController : ControllerBase
{
    private readonly AppPatinContext _db;
    public TutoresController(AppPatinContext db) => _db = db;

    public record TutorCreateDto(string Nombre, string Apellido, string? Telefono, string? Email, string? Relacion);
    public record TutorUpdateDto(string Nombre, string Apellido, string? Telefono, string? Email, string? Relacion);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? q)
    {
        var query = _db.Tutores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(t => t.Nombre.Contains(q) || t.Apellido.Contains(q) || (t.Email ?? "").Contains(q));

        var data = await query.OrderBy(t => t.Apellido).ThenBy(t => t.Nombre).ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Tutores.FindAsync(id);
        return t is null ? NotFound() : Ok(t);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TutorCreateDto dto)
    {
        var t = new Tutor
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Relacion = dto.Relacion
        };
        _db.Tutores.Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = t.TutorId }, t);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TutorUpdateDto dto)
    {
        var t = await _db.Tutores.FindAsync(id);
        if (t is null) return NotFound();

        t.Nombre = dto.Nombre;
        t.Apellido = dto.Apellido;
        t.Telefono = dto.Telefono;
        t.Email = dto.Email;
        t.Relacion = dto.Relacion;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tutores.FindAsync(id);
        if (t is null) return NotFound();
        _db.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
    
