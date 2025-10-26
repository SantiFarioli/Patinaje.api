using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Dashboard
{
    public record EventoDashboardDto(
        int TorneoId,
        string Nombre,
        string Lugar,
        DateTime FechaInicio,
        DateTime FechaFin,
        DateTime FechaLimiteInscripcion,
        string Organizador
    );
}
