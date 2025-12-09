using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
namespace Patinaje.API.Models
{
    public class Patinador
    {
        public int PatinadorId { get; set; }

        [MaxLength(50)]
        public string Nombre { get; set; } = null!;

        [MaxLength(50)]
        public string Apellido { get; set; } = null!;

        [MaxLength(10)]
        public string? Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }

        [MaxLength(50)]
        public string? Domicilio { get; set; }

        [MaxLength(100)]
        public string? FotoUrl { get; set; }

        [MaxLength(50)]
        public string Categoria { get; set; } = null!;
        public bool Activo { get; set; } = true;


        // Salud básica (extenderemos luego)

        [MaxLength(100)]
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
        public ICollection<EvaluacionTorneo> EvaluacionesTorneos { get; set; } = new List<EvaluacionTorneo>();
        public ICollection<EvaluacionTecnica> Evaluaciones { get; set; } = new List<EvaluacionTecnica>();

    }
}