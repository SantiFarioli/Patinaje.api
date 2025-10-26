using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.Models;

namespace Patinaje.API.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppPatinContext db)
    {
        await db.Database.MigrateAsync();

        // === Clubes ===
        var club1 = await db.Clubes.FirstOrDefaultAsync(c => c.ClubId == 1);
        if (club1 is null)
        {
            club1 = new Club { Nombre = "Club Estrella", Direccion = "Av. Principal 123", Telefono = "266-555555" };
            db.Clubes.Add(club1);
            await db.SaveChangesAsync();
        }

        var club2 = await db.Clubes.FirstOrDefaultAsync(c => c.ClubId == 2);
        if (club2 is null)
        {
            club2 = new Club { Nombre = "Club Aurora", Direccion = "Calle Secundaria 456", Telefono = "266-444444" };
            db.Clubes.Add(club2);
            await db.SaveChangesAsync();
        }

        // === Profesoras ===
        var profDemo = await db.Profesores.FirstOrDefaultAsync(p => p.Email == "profe@club.com");
        if (profDemo is null)
        {
            profDemo = new Profesor
            {
                Nombre = "Profe",
                Apellido = "Demo",
                Email = "profe@club.com",
                Telefono = "266-000000",
                Dni = "20111222",
                Domicilio = "Calle Falsa 123",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                ClubId = club1.ClubId
            };
            db.Profesores.Add(profDemo);
        }
        else
        {
            profDemo.Dni ??= "20111222";
            profDemo.Domicilio ??= "Calle Falsa 123";
            profDemo.ClubId = club1.ClubId;
        }
        await db.SaveChangesAsync();

        var profReal = await db.Profesores.FirstOrDefaultAsync(p => p.Email == "profe.agus@club.com");
        if (profReal is null)
        {
            profReal = new Profesor
            {
                Nombre = "Agustina",
                Apellido = "Luna",
                Email = "profe.agus@club.com",
                Telefono = "266-111111",
                Dni = "22333444",
                Domicilio = "Av. del Sol 456",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("patin2025"),
                ClubId = club2.ClubId
            };
            db.Profesores.Add(profReal);
        }
        else
        {
            profReal.Dni ??= "22333444";
            profReal.Domicilio ??= "Av. del Sol 456";
            profReal.ClubId = club2.ClubId;
        }
        await db.SaveChangesAsync();

        // === Patinadoras ===
        async Task<Patinador> EnsurePatinador(string nom, string ape, string dni, string dom, DateTime fnac, string cat, Profesor prof, Club club, int fotoSeed)
        {
            var p = await db.Patinadores.FirstOrDefaultAsync(x => x.Dni == dni);
            if (p is null)
            {
                p = new Patinador
                {
                    Nombre = nom,
                    Apellido = ape,
                    FechaNacimiento = fnac,
                    Categoria = cat,
                    Activo = true,
                    Dni = dni,
                    Domicilio = dom,
                    FichaMedica = "Apta",
                    AsisteGimnasio = true,
                    AsisteNutricionista = false,
                    AsistePsicologo = false,
                    ProfesorId = prof.ProfesorId,
                    ClubId = club.ClubId,
                    FotoUrl = $"https://picsum.photos/seed/{fotoSeed}/400"
                };
                db.Patinadores.Add(p);
            }
            else
            {
                p.Dni ??= dni;
                p.Domicilio ??= dom;
                if (string.IsNullOrEmpty(p.FotoUrl))
                    p.FotoUrl = $"https://picsum.photos/seed/{fotoSeed}/400";
            }
            await db.SaveChangesAsync();
            return p;
        }

        var cami = await EnsurePatinador("Camila", "Gómez", "45322111", "Barrio Sur 101", new DateTime(2012, 6, 10), "B Libre", profDemo, club1, 1);
        var sofi = await EnsurePatinador("Sofía", "Martínez", "46123999", "Calle Mitre 202", new DateTime(2011, 4, 25), "C Escuela", profDemo, club1, 2);
        var lucia = await EnsurePatinador("Lucía", "Pérez", "47222123", "Av. San Martín 789", new DateTime(2010, 9, 5), "A Libre", profReal, club2, 3);
        var lola = await EnsurePatinador("Lola", "Rivas", "48999123", "Los Álamos 333", new DateTime(2013, 1, 18), "B Escuela", profReal, club2, 4);

        // === Tutores ===
        async Task<Tutor> EnsureTutor(string nom, string ape, string dni, string dom, string email, string tel, string relacion)
        {
            var t = await db.Tutores.FirstOrDefaultAsync(x => x.Email == email);
            if (t is null)
            {
                t = new Tutor
                {
                    Nombre = nom,
                    Apellido = ape,
                    Dni = dni,
                    Domicilio = dom,
                    Email = email,
                    Telefono = tel,
                    Relacion = relacion
                };
                db.Tutores.Add(t);
            }
            else
            {
                t.Dni ??= dni;
                t.Domicilio ??= dom;
                t.Relacion ??= relacion;
            }
            await db.SaveChangesAsync();
            return t;
        }

        var tut1 = await EnsureTutor("Carolina", "Gómez", "30222333", "Barrio Norte 77", "caro.gomez@mail.com", "266-222222", "Madre");
        var tut2 = await EnsureTutor("Diego", "Martínez", "30111999", "Las Heras 55", "diego.mtz@mail.com", "266-333333", "Padre");

        // === Vinculos ===
        async Task EnsureVinculo(Tutor t, Patinador p)
        {
            var exists = await db.TutoresPatinadores.AnyAsync(x => x.TutorId == t.TutorId && x.PatinadorId == p.PatinadorId);
            if (!exists)
            {
                db.TutoresPatinadores.Add(new TutorPatinador { TutorId = t.TutorId, PatinadorId = p.PatinadorId });
                await db.SaveChangesAsync();
            }
        }

        await EnsureVinculo(tut1, cami);
        await EnsureVinculo(tut2, sofi);
        await EnsureVinculo(tut1, lucia);

        // === Torneos (ejemplo) ===
        var hoy = DateTime.Today;
        var futuros = new[] {
            new Torneo {
                Nombre = "Torneo Apertura",
                Lugar = "Club Unión",
                FechaInicio = hoy.AddDays(10).AddHours(9),
                FechaFin = hoy.AddDays(11).AddHours(18),
                FechaLimiteInscripcion = hoy.AddDays(5),
                Organizador = "Federación San Luis"
            },
            new Torneo {
                Nombre = "Selectivo Provincial",
                Lugar = "Pista Central",
                FechaInicio = hoy.AddDays(2).AddHours(18),
                FechaFin = hoy.AddDays(2).AddHours(21),
                FechaLimiteInscripcion = hoy.AddDays(1),
                Organizador = "Asociación Provincial"
            }
        };
        foreach (var t in futuros)
        {
            bool exists = await db.Torneos.AnyAsync(x => x.Nombre == t.Nombre && x.FechaInicio == t.FechaInicio);
            if (!exists) { db.Torneos.Add(t); await db.SaveChangesAsync(); }
        }

        // Torneo pasado
        var pasado = new Torneo
        {
            Nombre = "Copa Invierno",
            Lugar = "Club Norte",
            FechaInicio = hoy.AddDays(-20).AddHours(9),
            FechaFin = hoy.AddDays(-18).AddHours(18),
            FechaLimiteInscripcion = hoy.AddDays(-25),
            Organizador = "Liga Regional"
        };
        if (!await db.Torneos.AnyAsync(x => x.Nombre == pasado.Nombre && x.FechaInicio == pasado.FechaInicio))
        { db.Torneos.Add(pasado); await db.SaveChangesAsync(); }

        // === Asistencias ejemplo ===
        async Task EnsureAsistencia(Patinador p, DateTime fecha, bool presente)
        {
            bool exists = await db.Asistencias.AnyAsync(x => x.PatinadorId == p.PatinadorId && x.FechaClase == fecha.Date);
            if (!exists)
            {
                db.Asistencias.Add(new Asistencia { PatinadorId = p.PatinadorId, FechaClase = fecha.Date, Presente = presente });
                await db.SaveChangesAsync();
            }
        }
        await EnsureAsistencia(cami, hoy.AddDays(-2), true);
        await EnsureAsistencia(cami, hoy.AddDays(-1), true);
        await EnsureAsistencia(sofi, hoy.AddDays(-1), false);
        await EnsureAsistencia(lucia, hoy.AddDays(-3), true);

        // === Evaluaciones ejemplo ===
        async Task EnsureEval(Patinador p, string elemento, DateTime fecha, int puntaje, string? obs = null)
        {
            bool exists = await db.Evaluaciones.AnyAsync(x =>
                x.PatinadorId == p.PatinadorId && x.Elemento == elemento && x.Fecha == fecha);
            if (!exists)
            {
                db.Evaluaciones.Add(new EvaluacionTecnica
                {
                    PatinadorId = p.PatinadorId,
                    Elemento = elemento,
                    Fecha = fecha,
                    Puntaje = puntaje,
                    Observaciones = obs
                });
                await db.SaveChangesAsync();
            }
        }
        await EnsureEval(cami, "Salto Axel", hoy.AddDays(-7), 4, "Buena ejecución");
        await EnsureEval(sofi, "Giro Biellmann", hoy.AddDays(-5), 3, "Mejorar estabilidad");
        await EnsureEval(lucia, "Secuencia de pasos", hoy.AddDays(-3), 5, "Excelente ritmo");

        // === Pagos ejemplo ===
        async Task EnsurePago(Patinador p, string concepto, decimal monto, DateTime venc, string estado, DateTime? fechaPago = null)
        {
            bool exists = await db.Pagos.AnyAsync(x =>
                x.PatinadorId == p.PatinadorId && x.Concepto == concepto && x.FechaVencimiento == venc);
            if (!exists)
            {
                db.Pagos.Add(new Pago
                {
                    PatinadorId = p.PatinadorId,
                    Concepto = concepto,
                    Monto = monto,
                    Estado = estado,
                    FechaVencimiento = venc,
                    FechaPago = fechaPago
                });
                await db.SaveChangesAsync();
            }
        }
        await EnsurePago(cami, "Cuota Septiembre", 15000m, hoy.AddDays(9), "Pendiente");
        await EnsurePago(sofi, "Cuota Septiembre", 15000m, hoy.AddDays(9), "Pendiente");
        await EnsurePago(lucia, "Cuota Agosto", 15000m, hoy.AddDays(-20), "Pagado", hoy.AddDays(-15));
    }
}
