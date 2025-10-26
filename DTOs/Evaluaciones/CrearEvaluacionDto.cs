using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Evaluaciones
{
    public class CrearEvaluacionDto
    {
        [Required]
        public int PatinadorId { get; set; }

        [Required, MaxLength(50)]
        public string Elemento { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Range(1, 5)]
        public int Puntaje { get; set; }

        [MaxLength(200)]
        public string? Observaciones { get; set; }
    }
}
