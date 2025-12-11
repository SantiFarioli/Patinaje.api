namespace Patinaje.API.DTOs.Dashboard;

public record EventoDashboardDto(
    int TorneoId,
    string Nombre,
    string Lugar,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Organizador
);