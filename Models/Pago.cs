using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Pago
    {
        public int PagoId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        public string Concepto { get; set; } = null!; // Mensualidad, Torneo, Evento
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagado, Atrasado
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }
        public string? LinkComprobante { get; set; }
    }
}