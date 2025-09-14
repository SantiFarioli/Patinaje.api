using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Profesor
    {
        public int ProfesorId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Telefono { get; set; }

        public ICollection<Patinador> Patinadores { get; set; } = new List<Patinador>();

        public int? ClubId { get; set; }
        public Club? Club { get; set; }
    }
}