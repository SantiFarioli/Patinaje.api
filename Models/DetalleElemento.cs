using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.Models
{
    public class DetalleElemento
    {
        public int DetalleElementoId { get; set; }

        public int EvaluacionTorneoId { get; set; }
        public EvaluacionTorneo EvaluacionTorneo { get; set; } = null!;

        [MaxLength(50)]
        public string Elemento { get; set; } = string.Empty; // Ej: Axel, Trompo, Secuencia

        [MaxLength(50)]
        public string? CategoriaElemento { get; set; } // Ej: Saltos, Giros, Secuencias

        public int Puntaje { get; set; } // Valor jueces

        [MaxLength(300)]
        public string? Observaciones { get; set; } // Observación particular de ese elemento
    }
}
