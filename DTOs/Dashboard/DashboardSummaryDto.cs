namespace Patinaje.API.DTOs.Dashboard;

public record DashboardSummaryDto(
    int TotalPatinadoras,
    int TotalEventosProximos,
    int TotalPagosPendientes
);