using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Pagos
{
    public class MarcarPagoDto
    {
        [Required]
        public DateTime FechaPago { get; set; }

        [MaxLength(250)]
        public string? LinkComprobante { get; set; }
    }
}
