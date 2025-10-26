using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record AsistenciaDto(
    int AsistenciaId,
    DateTime FechaClase,
    bool Presente
);
