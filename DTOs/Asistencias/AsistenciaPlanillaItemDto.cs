
namespace Patinaje.API.DTOs.Asistencias

{
    public record AsistenciaPlanillaItemDto(
    int PatinadorId,
    string Nombre,
    string Apellido,
    string Categoria,
    bool Presente // Si es nueva, vendrá false. Si es edición, vendrá el valor real.
    );
}