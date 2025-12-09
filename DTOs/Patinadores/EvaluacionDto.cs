namespace Patinaje.API.DTOs.Patinadores;

public record EvaluacionDto(
    int EvaluacionId,
    string NombreTorneo,    // 👈 Nuevo
    DateTime Fecha,
    int Puntaje,            // Mantenemos por compatibilidad (aunque sea 0)
    string? Observaciones,
    string? ArchivoPdfUrl   // 👈 Nuevo
);