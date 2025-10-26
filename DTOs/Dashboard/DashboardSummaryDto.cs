using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Dashboard
{
    public record DashboardSummaryDto(int TotalPatinadoras, int TotalEventosProximos, int TotalPagosPendientes);
}
