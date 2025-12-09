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

        // POST: api/evaluacionestorneos
        // Usamos [FromForm] para permitir Multipart (Archivo + Datos)
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CrearEvaluacionTorneoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Validar existencia de Patinador y Torneo
            var patinador = await _db.Patinadores.FindAsync(request.PatinadorId);
            if (patinador is null) return BadRequest("Patinador no válido.");

            var torneo = await _db.Torneos.FindAsync(request.TorneoId);
            if (torneo is null) return BadRequest("Torneo no válido.");

            string? pdfUrl = null;

            // 2. Procesar el archivo PDF (si viene uno)
            if (request.ArchivoPdf != null && request.ArchivoPdf.Length > 0)
            {
                try 
                {
                    // Nombre único: eva_{id}_{uuid}.pdf
                    var fileName = $"eva_{request.PatinadorId}_{Guid.NewGuid()}.pdf";
                    
                    // Ruta física: wwwroot/uploads/evaluaciones
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "evaluaciones");
                    
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fullPath = Path.Combine(folderPath, fileName);

                    // Guardar en disco
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await request.ArchivoPdf.CopyToAsync(stream);
                    }

                    // Generar URL pública para guardar en BD
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    pdfUrl = $"{baseUrl}/uploads/evaluaciones/{fileName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error al subir archivo: {ex.Message}");
                }
            }

            // 3. Crear la entidad en la base de datos
            var eval = new EvaluacionTorneo
            {
                PatinadorId = request.PatinadorId,
                TorneoId = request.TorneoId,
                FechaEvaluacion = request.Fecha,
                ArchivoPdfUrl = pdfUrl,               // Guardamos la URL generada
                ObservacionesGenerales = request.Observaciones
            };

            _db.EvaluacionesTorneos.Add(eval);
            await _db.SaveChangesAsync();

            // Retornamos el objeto creado
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

        // GET: /api/evaluacionestorneos?patinadorId=123
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? patinadorId)
        {
            var q = _db.EvaluacionesTorneos.AsQueryable();
            if (patinadorId.HasValue)
                q = q.Where(x => x.PatinadorId == patinadorId.Value);

            var list = await q.OrderByDescending(x => x.FechaEvaluacion)
                              .ToListAsync();
            return Ok(list);
        }
    }
}