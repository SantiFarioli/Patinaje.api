using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record TutorDto(
    int TutorId,
    string Nombre,
    string Apellido,
    string? Dni,
    string? Domicilio,
    string? Telefono,
    string? Email,
    string? Relacion
);
