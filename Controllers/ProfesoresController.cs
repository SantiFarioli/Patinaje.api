using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfesoresController : ControllerBase
{
    private readonly AppPatinContext _db;
    public ProfesoresController(AppPatinContext db) => _db = db;

    // DTOs
    public record ProfesorCreateDto(string Nombre, string Apellido, string Email, string? Telefono, string Password, int? ClubId, string? Dni, string? Domicilio);
    public record ProfesorUpdateDto(string Nombre, string Apellido, string? Telefono, int? ClubId, string? Dni, string? Domicilio);
    public record ProfesorResponseDto(int ProfesorId, string Nombre, string Apellido, string Email, string? Telefono, int? ClubId, string? Dni, string? Domicilio);

    private static ProfesorResponseDto ToDto(Profesor p) =>
        new(p.ProfesorId, p.Nombre, p.Apellido, p.Email, p.Telefono, p.ClubId, p.Dni, p.Domicilio);

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? q)
    {
        var query = _db.Profesores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Nombre.Contains(q) || p.Apellido.Contains(q) || p.Email.Contains(q));

        var data = await query.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).Select(p => ToDto(p)).ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Profesores.FindAsync(id);
        return p is null ? NotFound() : Ok(ToDto(p));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProfesorCreateDto dto)
    {
        if (await _db.Profesores.AnyAsync(x => x.Email == dto.Email)) return Conflict("Email ya registrado");
        if (dto.ClubId.HasValue && !await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value)) return BadRequest("ClubId no válido");

        var entity = new Profesor
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email,
            Telefono = dto.Telefono,
            ClubId = dto.ClubId,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Dni = dto.Dni,
            Domicilio = dto.Domicilio
        };

        _db.Profesores.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.ProfesorId }, ToDto(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProfesorUpdateDto dto)
    {
        var p = await _db.Profesores.FindAsync(id);
        if (p is null) return NotFound();
        if (dto.ClubId.HasValue && !await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value)) return BadRequest("ClubId no válido");

        p.Nombre = dto.Nombre;
        p.Apellido = dto.Apellido;
        p.Telefono = dto.Telefono;
        p.ClubId = dto.ClubId;
        p.Dni = dto.Dni;
        p.Domicilio = dto.Domicilio;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] string password)
    {
        var p = await _db.Profesores.FindAsync(id);
        if (p is null) return NotFound();

        p.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Profesores.FindAsync(id);
        if (p is null) return NotFound();
        _db.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
