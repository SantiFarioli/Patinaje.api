using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Patinaje.API.Models;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppPatinContext _db;

    public AuthController(IConfiguration config, AppPatinContext db)
    {
        _config = config;
        _db = db;
    }

    public record LoginDto(string Email, string Password);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest("Email y contraseña son requeridos.");

        var profe = await _db.Profesores.FirstOrDefaultAsync(p => p.Email == dto.Email);
        if (profe is null) return Unauthorized("Usuario o contraseña inválidos.");

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, profe.PasswordHash);
        if (!ok) return Unauthorized("Usuario o contraseña inválidos.");

        var token = GenerarJwt(profe);
        return Ok(new
        {
            token,
            nombre = profe.Nombre,
            email = profe.Email,
            profesorId = profe.ProfesorId
        });
    }

    private string GenerarJwt(Profesor profe)
    {
        var jwt = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, profe.Email),
            new Claim("profesorId", profe.ProfesorId.ToString()),
            new Claim(ClaimTypes.Role, "Profesor")
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
