using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patinaje.API.DTOs.Patinadores;

public record PagoDto(
    int PagoId,
    string Concepto,
    decimal Monto,
    string Estado,
    DateTime? FechaVencimiento,
    DateTime? FechaPago
);
