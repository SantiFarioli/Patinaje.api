using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.EvaluacionesTorneos
{
    public class CrearEvaluacionTorneoDto
    {
        [Required]
        public int PatinadorId { get; set; }

        [Required]
        public int TorneoId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [MaxLength(250)]
        public string? ArchivoPdf { get; set; }

        [MaxLength(200)]
        public string? Observaciones { get; set; }
    }
}
