using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.Models
{
    public class EvaluacionTecnica
    {
        public int EvaluacionTecnicaId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        [MaxLength(50)]
        public string Elemento { get; set; } = string.Empty; // Ej: Axel, Trompo, Secuencia

        public DateTime Fecha { get; set; }

        public int Puntaje { get; set; }

        [MaxLength(50)]
        public string? Observaciones { get; set; } // Comentarios de la profe sobre la ejecución
    }
}
