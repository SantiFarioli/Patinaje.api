using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppPatinContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Profesores.AnyAsync())
        {
            db.Profesores.Add(new Profesor
            {
                Nombre = "Profe",
                Apellido = "Demo",
                Email = "profe@club.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Telefono = "266-000000"
            });
            await db.SaveChangesAsync();
        }
    }
}
