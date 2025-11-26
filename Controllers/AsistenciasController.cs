using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.DTOs.Asistencias;
using Patinaje.API.Models;

namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AsistenciasController : ControllerBase
    {
        private readonly AppPatinContext _db;

        public AsistenciasController(AppPatinContext db)
        {
            _db = db;
        }

        // GET: api/asistencias/planilla?fecha=2025-10-23
        // Devuelve la lista de patinadoras para tomar asistencia en esa fecha.
        [HttpGet("planilla")]
        public async Task<IActionResult> GetPlanilla([FromQuery] DateTime? fecha)
        {
            var dia = fecha?.Date ?? DateTime.Today;

            // 1. Buscar todas las patinadoras activas
            var patinadoras = await _db.Patinadores
                .Where(p => p.Activo)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

            // 2. Buscar si ya existen asistencias cargadas para ese día
            var asistenciasExistentes = await _db.Asistencias
                .Where(a => a.FechaClase == dia)
                .ToListAsync();

            // 3. Combinar (Left Join en memoria)
            var planilla = patinadoras.Select(p =>
            {
                // Buscamos si ya tiene asistencia registrada
                var asistencia = asistenciasExistentes.FirstOrDefault(a => a.PatinadorId == p.PatinadorId);
                
                // Si existe, usamos su valor. Si no, false por defecto.
                bool presente = asistencia != null && asistencia.Presente;

                return new AsistenciaPlanillaItemDto(
                    p.PatinadorId,
                    p.Nombre,
                    p.Apellido,
                    p.Categoria,
                    presente
                );
            });

            return Ok(planilla);
        }

        // POST: api/asistencias
        // Guarda o actualiza la asistencia masiva
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAsistenciaRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dia = request.Fecha.Date;

            // Traemos las asistencias que ya existan en la BD para ese día (para no duplicar)
            var existentes = await _db.Asistencias
                .Where(a => a.FechaClase == dia)
                .ToListAsync();

            foreach (var item in request.Detalles)
            {
                var existente = existentes.FirstOrDefault(a => a.PatinadorId == item.PatinadorId);

                if (existente != null)
                {
                    // Si ya existe, actualizamos
                    existente.Presente = item.Presente;
                }
                else
                {
                    // Si no existe, creamos nueva
                    var nueva = new Asistencia
                    {
                        PatinadorId = item.PatinadorId,
                        FechaClase = dia,
                        Presente = item.Presente
                    };
                    _db.Asistencias.Add(nueva);
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Asistencia guardada correctamente" });
        }
    }
}