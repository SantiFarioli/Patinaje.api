using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.Models
{
    public class EvaluacionTorneo
    {
        public int EvaluacionTorneoId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        public int TorneoId { get; set; }
        public Torneo Torneo { get; set; } = null!;

        [MaxLength(200)]
        public string? ArchivoPdfUrl { get; set; } // 📎 Link del PDF con planilla

        [MaxLength(500)]
        public string? ObservacionesGenerales { get; set; } // Comentarios globales del torneo

        public DateTime FechaEvaluacion { get; set; }

        // Relación 1:N con DetalleElemento
        public ICollection<DetalleElemento> Detalles { get; set; } = new List<DetalleElemento>();
    }
}
