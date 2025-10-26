using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Torneos
{
    public record TorneoUpdateDto(
        [Required, MaxLength(80)] string Nombre,
        [Required, MaxLength(80)] string Lugar,
        [Required] DateTime FechaInicio,
        [Required] DateTime FechaFin,
        [Required] DateTime FechaLimiteInscripcion,
        [Required, MaxLength(80)] string Organizador
    );
}
