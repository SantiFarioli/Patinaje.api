using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Pago
    {
        public int PagoId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        [MaxLength(100)]
        public string Concepto { get; set; } = null!; // Mensualidad, Torneo, Evento
        public decimal Monto { get; set; }

        [MaxLength(50)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Atrasado
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }

        [MaxLength(300)]
        public string? LinkComprobante { get; set; }
    }
}