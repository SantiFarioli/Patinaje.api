using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.DTOs.EvaluacionesTorneos;
using Patinaje.API.Models;

namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EvaluacionesTorneosController : ControllerBase
    {
        private readonly AppPatinContext _db;
        public EvaluacionesTorneosController(AppPatinContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearEvaluacionTorneoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patinador = await _db.Patinadores.FindAsync(dto.PatinadorId);
            if (patinador is null) return BadRequest("Patinador no válido.");

            var torneo = await _db.Torneos.FindAsync(dto.TorneoId);
            if (torneo is null) return BadRequest("Torneo no válido.");

            var eval = new EvaluacionTorneo
            {
                PatinadorId = dto.PatinadorId,
                TorneoId = dto.TorneoId,
                FechaEvaluacion = dto.Fecha,                     // 👈 map
                ArchivoPdfUrl = dto.ArchivoPdf,                  // 👈 map
                ObservacionesGenerales = dto.Observaciones       // 👈 map
            };

            _db.EvaluacionesTorneos.Add(eval);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = eval.EvaluacionTorneoId }, eval);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.EvaluacionesTorneos
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(x => x.EvaluacionTorneoId == id);

            return e is null ? NotFound() : Ok(e);
        }

        // /api/evaluacionestorneos?patinadorId=123
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? patinadorId)
        {
            var q = _db.EvaluacionesTorneos.AsQueryable();
            if (patinadorId.HasValue)
                q = q.Where(x => x.PatinadorId == patinadorId.Value);

            var list = await q.OrderByDescending(x => x.FechaEvaluacion) // 👈 propiedad nueva
                              .ToListAsync();
            return Ok(list);
        }
    }
}
