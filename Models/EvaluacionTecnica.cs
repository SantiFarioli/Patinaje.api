using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class EvaluacionTecnica
    {
        public int EvaluacionTecnicaId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        public string Elemento { get; set; } = null!; // ej: Salto Axel
        public DateTime Fecha { get; set; }
        public int Puntaje { get; set; } // 1 a 5
        public string? Observaciones { get; set; }
        public string? VideoUrl { get; set; }
    }
}