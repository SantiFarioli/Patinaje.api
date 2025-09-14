using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Tutor
    {
        public int TutorId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Relacion { get; set; } // Padre, Madre, etc.

        public ICollection<TutorPatinador> Patinadores { get; set; } = new List<TutorPatinador>();
    }
}