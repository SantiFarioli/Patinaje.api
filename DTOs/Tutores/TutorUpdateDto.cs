using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Tutores
{
    public record TutorUpdateDto(
        [Required, MaxLength(50)] string Nombre,
        [Required, MaxLength(50)] string Apellido,
        [MaxLength(50)] string? Telefono,
        [EmailAddress, MaxLength(120)] string? Email,
        [MaxLength(50)] string? Relacion,
        [MaxLength(20)] string? Dni,
        [MaxLength(100)] string? Domicilio
    );
}
