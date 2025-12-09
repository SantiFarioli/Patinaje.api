using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; 

namespace Patinaje.API.DTOs.EvaluacionesTorneos
{
    public class CrearEvaluacionTorneoRequest
    {
        [Required]
        public int PatinadorId { get; set; }

        [Required]
        public int TorneoId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public IFormFile? ArchivoPdf { get; set; }

        [MaxLength(200)]
        public string? Observaciones { get; set; }
    }
}