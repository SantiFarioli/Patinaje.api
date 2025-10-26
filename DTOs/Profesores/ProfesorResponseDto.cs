using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Profesores
{
    public record ProfesorResponseDto(
        int ProfesorId,
        string Nombre,
        string Apellido,
        string Email,
        string? Telefono,
        int? ClubId,
        string? Dni,
        string? Domicilio
    );
}
