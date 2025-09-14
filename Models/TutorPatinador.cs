using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class TutorPatinador
    {
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; } = null!;

        public int PatinadorId { get; set; }
        public Patinador Patinador { get; set; } = null!;
    }
}