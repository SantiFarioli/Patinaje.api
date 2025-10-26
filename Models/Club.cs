using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Club
    {
        public int ClubId { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(50)]
        public string? Direccion { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
        public ICollection<Patinador> Patinadores { get; set; } = new List<Patinador>();
    }
}