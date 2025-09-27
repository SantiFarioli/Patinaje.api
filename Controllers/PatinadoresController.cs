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

    // DTOs de entrada
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

    // DTO de salida (listado)
    public record PatinadorListDto(
        int PatinadorId,
        string Nombre,
        string Apellido,
        string Categoria,
        bool Activo
    );

    // DTO de detalle
    public record PatinadorDetailDto(
        int PatinadorId,
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
        string ProfesorNombre,
        int? ClubId,
        string? ClubNombre,
        List<TutorDto> Tutores // 👈 NUEVO
    );

    // DTO paginado
    public record PagedResult<T>(
        int TotalItems,
        int Page,
        int PageSize,
        IEnumerable<T> Data
    );


    public record TutorDto(
        int TutorId,
        string Nombre,
        string Apellido,
        string? Telefono,
        string? Email,
        string? Relacion
    );

    // GET /api/patinadores
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? search,
        [FromQuery] string? categoria,
        [FromQuery] bool? activo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("Parámetros de paginación inválidos.");

        var q = _db.Patinadores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            q = q.Where(p =>
                p.Nombre.ToLower().Contains(term) ||
                p.Apellido.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
            q = q.Where(p => p.Categoria == categoria);

        if (activo.HasValue)
            q = q.Where(p => p.Activo == activo.Value);

        var total = await q.CountAsync();

        var data = await q
            .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatinadorListDto(
                p.PatinadorId,
                p.Nombre,
                p.Apellido,
                p.Categoria,
                p.Activo
            ))
            .ToListAsync();

        var result = new PagedResult<PatinadorListDto>(
            total, page, pageSize, data
        );

        return Ok(result);
    }

    // GET /api/patinadores/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Patinadores
            .Include(p => p.Profesor)
            .Include(p => p.Club)
            .Where(p => p.PatinadorId == id)
            .Select(p => new PatinadorDetailDto(
                p.PatinadorId,
                p.Nombre,
                p.Apellido,
                p.FechaNacimiento,
                p.Categoria,
                p.Activo,
                p.FichaMedica,
                p.AsisteGimnasio,
                p.AsisteNutricionista,
                p.AsistePsicologo,
                p.ProfesorId,
                p.Profesor.Nombre + " " + p.Profesor.Apellido,
                p.ClubId,
                p.Club != null ? p.Club.Nombre : null,
                p.Tutores.Select(tp => new TutorDto(   // 👈 acá proyectamos los tutores
                tp.TutorId,
                tp.Tutor.Nombre,
                tp.Tutor.Apellido,
                tp.Tutor.Telefono,
                tp.Tutor.Email,
                tp.Tutor.Relacion
            )).ToList()
        ))
        .FirstOrDefaultAsync();

    return p is null ? NotFound("Patinador no encontrado.") : Ok(p);

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
