using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;


namespace Patinaje.API.Controllers
{
    [ApiController]
    [Route("eventos")]
    public class EventosController : ControllerBase
    {
        private readonly AppPatinContext _db;
        public EventosController(AppPatinContext db) => _db = db;


        // GET /eventos?limit=5  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> Get([FromQuery] int limit = 5)
        {
            var hoy = DateTime.Today;

            var raw = await _db.Torneos
                .Where(t => t.FechaFin >= hoy || t.FechaLimiteInscripcion >= hoy)
                .Select(t => new
                {
                    t.Nombre,
                    t.Lugar,
                    t.FechaInicio,
                    t.FechaFin,
                    t.FechaLimiteInscripcion
                })
                .ToListAsync();

            var list = raw
                .Select(t =>
                {
                    var nextKey =
                        (t.FechaInicio.Date >= hoy) ? t.FechaInicio :
                        (t.FechaLimiteInscripcion.Date >= hoy ? t.FechaLimiteInscripcion : t.FechaInicio);

                    var fechaStr = (t.FechaFin.Date > t.FechaInicio.Date)
                        ? $"{t.FechaInicio:yyyy-MM-dd HH:mm} - {t.FechaFin:yyyy-MM-dd HH:mm}"
                        : $"{t.FechaInicio:yyyy-MM-dd HH:mm}";

                    string? deadlineStr = (t.FechaLimiteInscripcion.Date >= hoy)
                        ? $"{t.FechaLimiteInscripcion:yyyy-MM-dd}"
                        : null;

                    return new
                    {
                        Next = nextKey,
                        Dto = new EventoDto
                        {
                            Titulo = t.Nombre,
                            Lugar  = t.Lugar,
                            Fecha  = fechaStr,
                            InscripcionHasta = deadlineStr
                        }
                    };
                })
                .OrderBy(x => x.Next)
                .Take(limit)
                .Select(x => x.Dto)
                .ToList();

            return Ok(list);
        }

        // DTO anidado
        public class EventoDto
        {
            public string Titulo { get; set; } = "";
            public string Fecha  { get; set; } = "";
            public string? Lugar { get; set; }
            public string? InscripcionHasta { get; set; }
        }
    }
}
