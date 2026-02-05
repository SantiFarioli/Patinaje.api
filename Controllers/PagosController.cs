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
    private readonly IWebHostEnvironment _env;

    public PagosController(AppPatinContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

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

    // POST: api/pagos/{id}/comprobante
    [HttpPost("{id:int}/comprobante")]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5MB max
    public async Task<IActionResult> UploadComprobante(int id, IFormFile file)
    {
        var pago = await _db.Pagos.FindAsync(id);
        if (pago is null) return NotFound("Pago no encontrado");
        
        if (file is null || file.Length == 0) 
            return BadRequest("Archivo no válido");

        // Validar extensión
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        if (!allowed.Contains(ext))
            return BadRequest("Formato no permitido. Use JPG, PNG o PDF.");

        try
        {
            var folderPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "comprobantes");
            Directory.CreateDirectory(folderPath);

            // Nombre: pago_{id}_{timestamp}.ext para evitar caché y duplicados
            var fileName = $"pago_{id}_{DateTime.Now.Ticks}{ext}";
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = $"{baseUrl}/uploads/comprobantes/{fileName}";

            pago.LinkComprobante = url;
            // Si suben comprobante, asumimos que se pagó? 
            // Mejor dejar que el cliente llame a 'MarcarPagado' o actualizar estado aqui:
            // pago.Estado = "Pagado"; // Opcional, depende del flujo. Lo dejo manual para no invadir.
            
            await _db.SaveChangesAsync();

            return Ok(new { pago.PagoId, LinkComprobante = url });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al subir comprobante: {ex.Message}");
        }
    }
}
