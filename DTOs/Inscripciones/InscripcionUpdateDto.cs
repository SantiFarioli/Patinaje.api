using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Inscripciones
{
    public class InscripcionUpdateDto
    {
        [Required, MaxLength(50)]
        public string CategoriaCompetencia { get; set; } = string.Empty;

        [Required]
        public decimal CostoInscripcion { get; set; }

        [Required, MaxLength(20)]
        public string EstadoPago { get; set; } = "Pendiente";

        [MaxLength(50)]
        public string? Resultado { get; set; }
    }
}
