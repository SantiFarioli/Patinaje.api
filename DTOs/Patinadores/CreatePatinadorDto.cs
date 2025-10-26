using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record CreatePatinadorDto(
    string Nombre,
    string Apellido,
    DateTime FechaNacimiento,
    string Categoria,
    bool Activo,
    string? Dni,
    string? Domicilio,
    string? FotoUrl,
    string? FichaMedica,
    bool AsisteGimnasio,
    bool AsisteNutricionista,
    bool AsistePsicologo,
    int ProfesorId,
    int? ClubId
);
