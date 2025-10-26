using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Profesor
    {
        public int ProfesorId { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        [MaxLength(10)]
        public string? Dni { get; set; }

        [MaxLength(50)]
        public string? Domicilio { get; set; }

        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MaxLength(300)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public ICollection<Patinador> Patinadores { get; set; } = new List<Patinador>();

        public int? ClubId { get; set; }
        public Club? Club { get; set; }
    }
}