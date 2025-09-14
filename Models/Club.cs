using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Club
    {
        public int ClubId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public ICollection<Patinador> Patinadores { get; set; } = new List<Patinador>();
    }
}