using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Profesores
{
    public record ProfesorCreateDto(
        [Required, MaxLength(50)] string Nombre,
        [Required, MaxLength(50)] string Apellido,
        [Required, EmailAddress, MaxLength(120)] string Email,
        [MaxLength(50)] string? Telefono,
        [Required, MinLength(6), MaxLength(100)] string Password,
        int? ClubId,
        [MaxLength(20)] string? Dni,
        [MaxLength(100)] string? Domicilio
    );
}
