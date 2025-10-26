using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record EvaluacionDto(
    int EvaluacionId,
    string Elemento,
    DateTime Fecha,
    int Puntaje,
    string? Observaciones
);
