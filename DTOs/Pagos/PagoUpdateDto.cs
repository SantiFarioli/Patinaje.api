using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Pagos
{
    public class PagoUpdateDto
    {
        [Required, MaxLength(50)]
        public string Concepto { get; set; } = string.Empty;

        [Required]
        public decimal Monto { get; set; }

        [Required, MaxLength(20)]
        public string Estado { get; set; } = string.Empty;

        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }

        [MaxLength(250)]
        public string? LinkComprobante { get; set; }
    }
}
