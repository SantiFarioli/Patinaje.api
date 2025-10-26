using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.DTOs.DetallesElementos;
using Patinaje.API.Models;

namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DetallesElementosController : ControllerBase
    {
        private readonly AppPatinContext _db;
        public DetallesElementosController(AppPatinContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearDetalleElementoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var eval = await _db.EvaluacionesTorneos.FindAsync(dto.EvaluacionTorneoId);
            if (eval is null) return BadRequest("EvaluacionTorneoId no válido.");

            var detalle = new DetalleElemento
            {
                EvaluacionTorneoId = dto.EvaluacionTorneoId,
                Elemento = dto.Elemento,
                Puntaje = dto.Puntaje,                      // 👈 ahora int, compila
                Observaciones = dto.Observaciones
            };

            _db.DetallesElementos.Add(detalle);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = detalle.DetalleElementoId }, detalle);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var d = await _db.DetallesElementos.FindAsync(id);
            return d is null ? NotFound() : Ok(d);
        }
    }
}
