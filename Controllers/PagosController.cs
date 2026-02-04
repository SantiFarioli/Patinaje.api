using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;
using Patinaje.API.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly AppPatinContext _db;
    public PagosController(AppPatinContext db) => _db = db;

    public record PagoCreateDto(
        int PatinadorId,
        string Concepto,           // "Cuota Sep/2025", "Inscripción Torneo X", etc.
        decimal Monto,
        string Estado,             // "Pendiente" | "Pagado"
        DateTime FechaVencimiento,
        string? LinkComprobante
    );

    public record PagoUpdateDto(
        string Concepto,
        decimal Monto,
        string Estado,
        DateTime? FechaVencimiento,
        DateTime? FechaPago,
        string? LinkComprobante
    );

    // DTO específico para esta lista (defínelo aquí mismo o arriba junto a los otros)
    public record PagoPendienteDto(
        int PagoId,
        string PatinadoraNombre,  // 👈 Dato clave
        string Concepto,
        decimal Monto,
        DateTime FechaVencimiento
    );

    public record MarcarPagoDto(DateTime FechaPago, string? LinkComprobante);

    // GET /api/pagos?patinadorId=1&estado=Pendiente
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? patinadorId, [FromQuery] string? estado)
    {
        var q = _db.Pagos.AsQueryable();
        if (patinadorId.HasValue) q = q.Where(p => p.PatinadorId == patinadorId.Value);
        if (!string.IsNullOrWhiteSpace(estado)) q = q.Where(p => p.Estado == estado);

        var data = await q.OrderByDescending(p => p.FechaVencimiento ?? p.FechaPago)
                          .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Pagos.FindAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PagoCreateDto dto)
    {
        var existe = await _db.Patinadores.AnyAsync(p => p.PatinadorId == dto.PatinadorId);
        if (!existe) return BadRequest("PatinadorId no válido.");

        var entity = new Pago
        {
            PatinadorId = dto.PatinadorId,
            Concepto = dto.Concepto,
            Monto = dto.Monto,
            Estado = dto.Estado,
            FechaVencimiento = dto.FechaVencimiento,
            FechaPago = null,
            LinkComprobante = dto.LinkComprobante
        };

        _db.Pagos.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.PagoId }, entity);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PagoUpdateDto dto)
    {
        var entity = await _db.Pagos.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Concepto = dto.Concepto;
        entity.Monto = dto.Monto;
        entity.Estado = dto.Estado;
        entity.FechaVencimiento = dto.FechaVencimiento;
        entity.FechaPago = dto.FechaPago;
        entity.LinkComprobante = dto.LinkComprobante;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT /api/pagos/{id}/pagar
    [HttpPut("{id:int}/pagar")]
    public async Task<IActionResult> MarcarPagado(int id, [FromBody] MarcarPagoDto dto)
    {
        var p = await _db.Pagos.FindAsync(id);
        if (p is null) return NotFound();

        p.Estado = "Pagado";
        p.FechaPago = dto.FechaPago;
        p.LinkComprobante = dto.LinkComprobante ?? p.LinkComprobante;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Pagos.FindAsync(id);
        if (p is null) return NotFound();
        _db.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET: api/pagos/pendientes
    // Devuelve todos los pagos que NO están pagados, con el nombre de la chica
    [HttpGet("pendientes")]
    public async Task<IActionResult> GetPendientes()
    {
        var lista = await _db.Pagos
            .Include(p => p.Patinador) // Importante: Join con Patinador
            .Where(p => p.Estado == "Pendiente")
            .OrderBy(p => p.FechaVencimiento) // Los más urgentes primero
            .Select(p => new PagoPendienteDto(
                p.PagoId,
                p.Patinador.Nombre + " " + p.Patinador.Apellido,
                p.Concepto,
                p.Monto,
                p.FechaVencimiento ?? DateTime.MaxValue
            ))
            .ToListAsync();

        return Ok(lista);
    }
}
