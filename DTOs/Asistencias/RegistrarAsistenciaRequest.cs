using System.ComponentModel.DataAnnotations;

namespace Patinaje.API.DTOs.Asistencias;

public class RegistrarAsistenciaRequest
{
    [Required]
    public DateTime Fecha { get; set; }

    // Lista simple: ID del patinador y si vino o no
    public List<AsistenciaUpdateItem> Detalles { get; set; } = new();
}

public record AsistenciaUpdateItem(int PatinadorId, bool Presente);