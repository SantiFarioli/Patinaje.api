using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Torneo
    {
        public int TorneoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Lugar { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaLimiteInscripcion { get; set; }
        public string Organizador { get; set; } = null!;

        // Relaciones
        public ICollection<InscripcionTorneo> Inscripciones { get; set; } = new List<InscripcionTorneo>();
    }
}