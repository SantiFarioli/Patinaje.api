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
        private readonly IWebHostEnvironment _env;

        public EvaluacionesTorneosController(AppPatinContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // POST: api/evaluacionestorneos
        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)] // Límite de 10MB para todo el request
        public async Task<IActionResult> Create([FromForm] CrearEvaluacionTorneoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Validar existencia de Patinador y Torneo
            var patinador = await _db.Patinadores.FindAsync(request.PatinadorId);
            if (patinador is null) return BadRequest("Patinador no válido.");

            var torneo = await _db.Torneos.FindAsync(request.TorneoId);
            if (torneo is null) return BadRequest("Torneo no válido.");

            // 2. Regla de Negocio: Validar Fecha (+/- 7 días del torneo)
            var fechaInicio = torneo.FechaInicio;
            // No hay fecha fin en el modelo actual? Usamos inicio como referencia
            // Si quieres ser estricto: FechaInicio.AddDays(-7) <= request.Fecha <= FechaFin.AddDays(7)
            // Asumiendo Fecha evaluacion alrededor de Fecha Inicio:
            var diff = (request.Fecha - fechaInicio).TotalDays;
            if (Math.Abs(diff) > 7)
            {
                return BadRequest($"La fecha de evaluación debe estar dentro de los 7 días cercanos al inicio del torneo ({fechaInicio:dd/MM/yyyy}).");
            }

            // 3. Crear Entidad (sin PDF aún para tener el ID)
            var eval = new EvaluacionTorneo
            {
                PatinadorId = request.PatinadorId,
                TorneoId = request.TorneoId,
                FechaEvaluacion = request.Fecha,
                ArchivoPdfUrl = null,
                ObservacionesGenerales = request.Observaciones
            };

            // Usamos transacción para asegurar consistencia
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.EvaluacionesTorneos.Add(eval);
                await _db.SaveChangesAsync(); // Obtenemos ID aquí

                // 4. Procesar el archivo PDF (si existe)
                if (request.ArchivoPdf != null)
                {
                    if (request.ArchivoPdf.Length > 10 * 1024 * 1024)
                        throw new Exception("El archivo excede el límite de 10MB.");

                    if (request.ArchivoPdf.ContentType != "application/pdf")
                         throw new Exception("Solo se permiten archivos PDF.");

                    // Nombre estándar: eva_{EvaluacionId}.pdf
                    var fileName = $"eva_{eval.EvaluacionTorneoId}.pdf";
                    
                    var folderPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "evaluaciones");
                    Directory.CreateDirectory(folderPath);

                    var fullPath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await request.ArchivoPdf.CopyToAsync(stream);
                    }

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    eval.ArchivoPdfUrl = $"{baseUrl}/uploads/evaluaciones/{fileName}";
                    
                    await _db.SaveChangesAsync(); // Actualizamos URL
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al guardar evaluación: {ex.Message}");
            }

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