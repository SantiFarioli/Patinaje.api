using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Tutor
    {
        public int TutorId { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        [MaxLength(10)]
        public string? Dni { get; set; }

        [MaxLength(100)]
        public string? Domicilio { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Relacion { get; set; } // Padre, Madre, etc.

        public ICollection<TutorPatinador> Patinadores { get; set; } = new List<TutorPatinador>();
    }
}