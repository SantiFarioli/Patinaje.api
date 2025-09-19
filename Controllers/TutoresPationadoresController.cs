using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TutoresPatinadoresController : ControllerBase
{
    private readonly AppPatinContext _db;
    public TutoresPatinadoresController(AppPatinContext db) => _db = db;

    public record VincularDto(int TutorId, int PatinadorId);

    // POST /api/tutorespatinadores  (vincula)
    [HttpPost]
    public async Task<IActionResult> Vincular([FromBody] VincularDto dto)
    {
        var tOk = await _db.Tutores.AnyAsync(t => t.TutorId == dto.TutorId);
        var pOk = await _db.Patinadores.AnyAsync(p => p.PatinadorId == dto.PatinadorId);
        if (!tOk || !pOk) return BadRequest("TutorId/PatinadorId inválidos.");

        var existe = await _db.TutoresPatinadores
            .AnyAsync(x => x.TutorId == dto.TutorId && x.PatinadorId == dto.PatinadorId);
        if (existe) return Conflict("Ya están vinculados.");

        _db.TutoresPatinadores.Add(new TutorPatinador { TutorId = dto.TutorId, PatinadorId = dto.PatinadorId });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/tutorespatinadores?tutorId=1&patinadorId=2  (desvincula)
    [HttpDelete]
    public async Task<IActionResult> Desvincular([FromQuery] int tutorId, [FromQuery] int patinadorId)
    {
        var link = await _db.TutoresPatinadores
            .FirstOrDefaultAsync(x => x.TutorId == tutorId && x.PatinadorId == patinadorId);

        if (link is null) return NotFound();

        _db.TutoresPatinadores.Remove(link);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/tutorespatinadores?patinadorId=1  (ver tutores del patinador) 
    // o /api/tutorespatinadores?tutorId=1      (ver patinadores de un tutor)
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? patinadorId, [FromQuery] int? tutorId)
    {
        if (patinadorId.HasValue)
        {
            var data = await _db.TutoresPatinadores
                .Where(x => x.PatinadorId == patinadorId.Value)
                .Include(x => x.Tutor)
                .Select(x => x.Tutor)
                .OrderBy(t => t.Apellido).ThenBy(t => t.Nombre)
                .ToListAsync();
            return Ok(data);
        }
        if (tutorId.HasValue)
        {
            var data = await _db.TutoresPatinadores
                .Where(x => x.TutorId == tutorId.Value)
                .Include(x => x.Patinador)
                .Select(x => x.Patinador)
                .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
                .ToListAsync();
            return Ok(data);
        }
        return BadRequest("Debes enviar tutorId o patinadorId.");
    }
}
