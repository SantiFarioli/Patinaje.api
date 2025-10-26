using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Profesores
{
    public record ProfesorUpdateDto(
        [Required, MaxLength(50)] string Nombre,
        [Required, MaxLength(50)] string Apellido,
        [MaxLength(50)] string? Telefono,
        int? ClubId,
        [MaxLength(20)] string? Dni,
        [MaxLength(100)] string? Domicilio
    );
}
