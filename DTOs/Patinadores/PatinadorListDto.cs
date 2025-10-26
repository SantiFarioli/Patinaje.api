using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record PatinadorListDto(
    int PatinadorId,
    string Nombre,
    string Apellido,
    string Categoria,
    bool Activo,
    string? FotoUrl
);
