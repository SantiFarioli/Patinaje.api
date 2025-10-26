using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
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
        [MaxLength(300)]
        public string CategoriaCompetencia { get; set; } = null!;
        public decimal CostoInscripcion { get; set; }
        
        [MaxLength(300)]
        public string EstadoPago { get; set; } = "Pendiente"; // Pendiente, Pagado
        [MaxLength(300)]
        public string? Resultado { get; set; } // Puesto o puntaje
    }
}