using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;
using Patinaje.API.Data;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClubesController : ControllerBase
{
    private readonly AppPatinContext _db;
    public ClubesController(AppPatinContext db) => _db = db;

    // GET /api/clubes
    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _db.Clubes.OrderBy(c => c.Nombre).ToListAsync());

    // GET /api/clubes/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var club = await _db.Clubes.FindAsync(id);
        return club is null ? NotFound() : Ok(club);
    }

    // POST /api/clubes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Club dto)
    {
        _db.Clubes.Add(dto);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dto.ClubId }, dto);
    }

    // PUT /api/clubes/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Club dto)
    {
        var club = await _db.Clubes.FindAsync(id);
        if (club is null) return NotFound();

        club.Nombre = dto.Nombre;
        club.Direccion = dto.Direccion;
        club.Telefono = dto.Telefono;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/clubes/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var club = await _db.Clubes.FindAsync(id);
        if (club is null) return NotFound();

        _db.Remove(club);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
