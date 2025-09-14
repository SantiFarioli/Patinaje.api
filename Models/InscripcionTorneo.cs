using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class InscripcionTorneo
    {
        public int InscripcionTorneoId { get; set; }

        // FK -> Torneo
        public int TorneoId { get; set; }
        public Torneo Torneo { get; set; } = null!;

        // FK -> Patinador
        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        // Datos adicionales
        public string CategoriaCompetencia { get; set; } = null!;
        public decimal CostoInscripcion { get; set; }
        public string EstadoPago { get; set; } = "Pendiente"; // Pendiente, Pagado
        public string? Resultado { get; set; } // Puesto o puntaje
    }
}