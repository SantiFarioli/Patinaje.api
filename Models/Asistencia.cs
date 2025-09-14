using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Asistencia
    {
        public int AsistenciaId { get; set; }

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;

        public DateTime FechaClase { get; set; }
        public bool Presente { get; set; }
    }
}