using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatinadoresController : ControllerBase
{
    private readonly AppPatinContext _db;
    public PatinadoresController(AppPatinContext db) => _db = db;

    public record CreatePatinadorDto(
        string Nombre,
        string Apellido,
        DateTime FechaNacimiento,
        string Categoria,
        bool Activo,
        string? FichaMedica,
        bool AsisteGimnasio,
        bool AsisteNutricionista,
        bool AsistePsicologo,
        int ProfesorId,
        int? ClubId
    );

    public record UpdatePatinadorDto(
        string Nombre,
        string Apellido,
        DateTime FechaNacimiento,
        string Categoria,
        bool Activo,
        string? FichaMedica,
        bool AsisteGimnasio,
        bool AsisteNutricionista,
        bool AsistePsicologo,
        int ProfesorId,
        int? ClubId
    );

    // GET /api/patinadores?search=&categoria=&activo=true&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] string? categoria,
        [FromQuery] bool? activo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var q = _db.Patinadores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p => p.Nombre.Contains(search) || p.Apellido.Contains(search));

        if (!string.IsNullOrWhiteSpace(categoria))
            q = q.Where(p => p.Categoria == categoria);

        if (activo.HasValue)
            q = q.Where(p => p.Activo == activo.Value);

        var total = await q.CountAsync();
        var data = await q
            .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, data });
    }

    // GET /api/patinadores/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Patinadores.FindAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    // POST /api/patinadores
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatinadorDto dto)
    {
        var existeProfe = await _db.Profesores.AnyAsync(p => p.ProfesorId == dto.ProfesorId);
        if (!existeProfe) return BadRequest("ProfesorId no válido.");

        if (dto.ClubId.HasValue)
        {
            var existeClub = await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value);
            if (!existeClub) return BadRequest("ClubId no válido.");
        }

        var entity = new Patinador
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            FechaNacimiento = dto.FechaNacimiento,
            Categoria = dto.Categoria,
            Activo = dto.Activo,
            FichaMedica = dto.FichaMedica,
            AsisteGimnasio = dto.AsisteGimnasio,
            AsisteNutricionista = dto.AsisteNutricionista,
            AsistePsicologo = dto.AsistePsicologo,
            ProfesorId = dto.ProfesorId,
            ClubId = dto.ClubId
        };

        _db.Patinadores.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.PatinadorId }, entity);
    }

    // PUT /api/patinadores/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatinadorDto dto)
    {
        var entity = await _db.Patinadores.FindAsync(id);
        if (entity is null) return NotFound();

        var existeProfe = await _db.Profesores.AnyAsync(p => p.ProfesorId == dto.ProfesorId);
        if (!existeProfe) return BadRequest("ProfesorId no válido.");

        if (dto.ClubId.HasValue)
        {
            var existeClub = await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value);
            if (!existeClub) return BadRequest("ClubId no válido.");
        }

        entity.Nombre = dto.Nombre;
        entity.Apellido = dto.Apellido;
        entity.FechaNacimiento = dto.FechaNacimiento;
        entity.Categoria = dto.Categoria;
        entity.Activo = dto.Activo;
        entity.FichaMedica = dto.FichaMedica;
        entity.AsisteGimnasio = dto.AsisteGimnasio;
        entity.AsisteNutricionista = dto.AsisteNutricionista;
        entity.AsistePsicologo = dto.AsistePsicologo;
        entity.ProfesorId = dto.ProfesorId;
        entity.ClubId = dto.ClubId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/patinadores/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Patinadores.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
