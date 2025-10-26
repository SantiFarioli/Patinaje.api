using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.DetallesElementos
{
    public class CrearDetalleElementoDto
    {
        [Required]
        public int EvaluacionTorneoId { get; set; }

        [Required, MaxLength(50)] 
        public string Elemento { get; set; } = string.Empty;
        [Required, Range(0, 10)]
        public int Puntaje { get; set; }  
        [MaxLength(300)] 
        public string? Observaciones { get; set; } 
    }
}
