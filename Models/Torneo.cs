using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Torneo
    {
        public int TorneoId { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(100)]
        public string Lugar { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaLimiteInscripcion { get; set; }

        [MaxLength(100)]
        public string Organizador { get; set; } = null!;

        // Relaciones
        public ICollection<InscripcionTorneo> Inscripciones { get; set; } = new List<InscripcionTorneo>();
    }
}