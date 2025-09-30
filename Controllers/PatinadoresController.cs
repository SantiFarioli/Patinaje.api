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
    private readonly IWebHostEnvironment _env;

    public PatinadoresController(AppPatinContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // ===== DTOs de entrada =====
    public record CreatePatinadorDto(
        string Nombre,
        string Apellido,
        DateTime FechaNacimiento,
        string Categoria,
        bool Activo,
        string? Dni,
        string? Domicilio,
        string? FotoUrl,
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
        string? Dni,
        string? Domicilio,
        string? FotoUrl,
        string? FichaMedica,
        bool AsisteGimnasio,
        bool AsisteNutricionista,
        bool AsistePsicologo,
        int ProfesorId,
        int? ClubId
    );

    // ===== DTOs de salida =====
    public record PatinadorListDto(
        int PatinadorId,
        string Nombre,
        string Apellido,
        string Categoria,
        bool Activo,
        string? FotoUrl
    );

    public record TutorDto(
        int TutorId,
        string Nombre,
        string Apellido,
        string? Dni,
        string? Domicilio,
        string? Telefono,
        string? Email,
        string? Relacion
    );

    public record PatinadorDetailDto(
        int PatinadorId,
        string Nombre,
        string Apellido,
        DateTime FechaNacimiento,
        string Categoria,
        bool Activo,
        string? Dni,
        string? Domicilio,
        string? FotoUrl,
        string? FichaMedica,
        bool AsisteGimnasio,
        bool AsisteNutricionista,
        bool AsistePsicologo,
        int ProfesorId,
        string ProfesorNombre,
        int? ClubId,
        string? ClubNombre,
        List<TutorDto> Tutores
    );

    public record PagedResult<T>(int TotalItems, int Page, int PageSize, IEnumerable<T> Data);

    // ===== GET list =====
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? categoria,
                                         [FromQuery] bool? activo, [FromQuery] int page = 1,
                                         [FromQuery] int pageSize = 20)
    {
        if (page <= 0 || pageSize <= 0) return BadRequest("Paginación inválida");

        var q = _db.Patinadores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            q = q.Where(p =>
                p.Nombre.ToLower().Contains(term) ||
                p.Apellido.ToLower().Contains(term) ||
                (p.Dni ?? "").ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
            q = q.Where(p => p.Categoria == categoria);

        if (activo.HasValue)
            q = q.Where(p => p.Activo == activo.Value);

        var total = await q.CountAsync();

        var data = await q.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatinadorListDto(
                p.PatinadorId, p.Nombre, p.Apellido, p.Categoria, p.Activo, p.FotoUrl))
            .ToListAsync();

        return Ok(new PagedResult<PatinadorListDto>(total, page, pageSize, data));
    }

    // ===== GET detail =====
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Patinadores
            .Include(x => x.Profesor)
            .Include(x => x.Club)
            .Include(x => x.Tutores).ThenInclude(tp => tp.Tutor)
            .Where(x => x.PatinadorId == id)
            .Select(p => new PatinadorDetailDto(
                p.PatinadorId,
                p.Nombre,
                p.Apellido,
                p.FechaNacimiento,
                p.Categoria,
                p.Activo,
                p.Dni,
                p.Domicilio,
                p.FotoUrl,
                p.FichaMedica,
                p.AsisteGimnasio,
                p.AsisteNutricionista,
                p.AsistePsicologo,
                p.ProfesorId,
                p.Profesor.Nombre + " " + p.Profesor.Apellido,
                p.ClubId,
                p.Club != null ? p.Club.Nombre : null,
                p.Tutores.Select(tp => new TutorDto(
                    tp.TutorId,
                    tp.Tutor.Nombre,
                    tp.Tutor.Apellido,
                    tp.Tutor.Dni,
                    tp.Tutor.Domicilio,
                    tp.Tutor.Telefono,
                    tp.Tutor.Email,
                    tp.Tutor.Relacion
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        return p is null ? NotFound("Patinador no encontrado") : Ok(p);
    }

    // ===== POST =====
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatinadorDto dto)
    {
        if (!await _db.Profesores.AnyAsync(p => p.ProfesorId == dto.ProfesorId))
            return BadRequest("ProfesorId no válido");

        if (dto.ClubId.HasValue && !await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value))
            return BadRequest("ClubId no válido");

        var entity = new Patinador
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            FechaNacimiento = dto.FechaNacimiento,
            Categoria = dto.Categoria,
            Activo = dto.Activo,
            Dni = dto.Dni,
            Domicilio = dto.Domicilio,
            FotoUrl = dto.FotoUrl,
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

    // ===== PUT =====
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePatinadorDto dto)
    {
        var entity = await _db.Patinadores.FindAsync(id);
        if (entity is null) return NotFound();

        if (!await _db.Profesores.AnyAsync(p => p.ProfesorId == dto.ProfesorId))
            return BadRequest("ProfesorId no válido");

        if (dto.ClubId.HasValue && !await _db.Clubes.AnyAsync(c => c.ClubId == dto.ClubId.Value))
            return BadRequest("ClubId no válido");

        entity.Nombre = dto.Nombre;
        entity.Apellido = dto.Apellido;
        entity.FechaNacimiento = dto.FechaNacimiento;
        entity.Categoria = dto.Categoria;
        entity.Activo = dto.Activo;
        entity.Dni = dto.Dni;
        entity.Domicilio = dto.Domicilio;
        entity.FotoUrl = dto.FotoUrl;
        entity.FichaMedica = dto.FichaMedica;
        entity.AsisteGimnasio = dto.AsisteGimnasio;
        entity.AsisteNutricionista = dto.AsisteNutricionista;
        entity.AsistePsicologo = dto.AsistePsicologo;
        entity.ProfesorId = dto.ProfesorId;
        entity.ClubId = dto.ClubId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ===== DELETE =====
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Patinadores.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ===== Foto upload =====
    [HttpPost("{id:int}/foto")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadFoto(int id, IFormFile file)
    {
        var pat = await _db.Patinadores.FindAsync(id);
        if (pat is null) return NotFound();
        if (file is null || file.Length == 0) return BadRequest("Archivo inválido");

        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "patinadoras");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";

        var fileName = $"{id}{ext}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        using (var stream = System.IO.File.Create(fullPath))
            await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        pat.FotoUrl = $"{baseUrl}/uploads/patinadoras/{fileName}";

        await _db.SaveChangesAsync();
        return Ok(new { pat.PatinadorId, pat.FotoUrl });
    }
}
