using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record PatinadorDetailDto(
    int PatinadorId,
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
    string ProfesorNombre,
    int? ClubId,
    string? ClubNombre,
    List<TutorDto> Tutores,
    List<AsistenciaDto> Asistencias,
    List<PagoDto> Pagos,
    List<EvaluacionDto> Evaluaciones
);
