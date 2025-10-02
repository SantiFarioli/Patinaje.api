using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.Models
{
    public class Patinador
    {
        public int PatinadorId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Domicilio { get; set; }
        public string? FotoUrl { get; set; }
        public string Categoria { get; set; } = null!;
        public bool Activo { get; set; } = true;


        // Salud básica (extenderemos luego)
        public string? FichaMedica { get; set; }
        public bool AsisteGimnasio { get; set; }
        public bool AsisteNutricionista { get; set; }
        public bool AsistePsicologo { get; set; }

        // FK -> Profesor
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; } = null!;
        public int? ClubId { get; set; }
        public Club? Club { get; set; }


        // M:N con Tutor
        public ICollection<TutorPatinador> Tutores { get; set; } = new List<TutorPatinador>();
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<EvaluacionTecnica> Evaluaciones { get; set; } = new List<EvaluacionTecnica>();
        
    }
}