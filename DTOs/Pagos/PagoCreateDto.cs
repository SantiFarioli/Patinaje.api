using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Pagos
{
    public class PagoCreateDto
    {
        [Required]
        public int PatinadorId { get; set; }

        [Required, MaxLength(50)]
        public string Concepto { get; set; } = string.Empty;

        [Required]
        public decimal Monto { get; set; }

        [Required, MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(250)]
        public string? LinkComprobante { get; set; }
    }
}
