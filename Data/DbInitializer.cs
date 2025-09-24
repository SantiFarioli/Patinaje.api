using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppPatinContext db)
    {
        await db.Database.MigrateAsync();

        // === Club ===
        var club = await db.Clubes.FirstOrDefaultAsync(c => c.Nombre == "Club Estrella");
        if (club is null)
        {
            club = new Club { Nombre = "Club Estrella", Direccion = "Av. Principal 123", Telefono = "266-555555" };
            db.Clubes.Add(club);
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                ClubId = club.ClubId
            };
            db.Profesores.Add(profDemo);
            await db.SaveChangesAsync();
        }

        var profReal = await db.Profesores.FirstOrDefaultAsync(p => p.Email == "profe.agus@club.com");
        if (profReal is null)
        {
            profReal = new Profesor
            {
                Nombre = "Agustina",
                Apellido = "Luna",
                Email = "profe.agus@club.com",
                Telefono = "266-111111",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("patin2025"),
                ClubId = club.ClubId
            };
            db.Profesores.Add(profReal);
            await db.SaveChangesAsync();
        }

        // === Patinadoras (4) ===
        async Task<Patinador> EnsurePatinador(string nom, string ape, DateTime fnac, string cat, Profesor prof)
        {
            var p = await db.Patinadores.FirstOrDefaultAsync(x =>
                x.Nombre == nom && x.Apellido == ape && x.FechaNacimiento == fnac);
            if (p is null)
            {
                p = new Patinador
                {
                    Nombre = nom,
                    Apellido = ape,
                    FechaNacimiento = fnac,
                    Categoria = cat,
                    Activo = true,
                    FichaMedica = "Apta",
                    AsisteGimnasio = true,
                    AsisteNutricionista = false,
                    AsistePsicologo = false,
                    ProfesorId = prof.ProfesorId,
                    ClubId = club.ClubId
                };
                db.Patinadores.Add(p);
                await db.SaveChangesAsync();
            }
            return p;
        }

        var cami  = await EnsurePatinador("Camila", "Gómez",  new DateTime(2012,6,10), "B Libre", profDemo);
        var sofi  = await EnsurePatinador("Sofía", "Martínez",new DateTime(2011,4,25), "C Escuela", profDemo);
        var lucia = await EnsurePatinador("Lucía", "Pérez",   new DateTime(2010,9, 5), "A Libre",  profReal);
        var lola  = await EnsurePatinador("Lola", "Rivas",    new DateTime(2013,1,18), "B Escuela",profReal);

        // === Tutores + vínculos M:N ===
        async Task<Tutor> EnsureTutor(string nom, string ape, string email, string tel)
        {
            var t = await db.Tutores.FirstOrDefaultAsync(x => x.Email == email);
            if (t is null)
            {
                t = new Tutor { Nombre = nom, Apellido = ape, Email = email, Telefono = tel };
                db.Tutores.Add(t);
                await db.SaveChangesAsync();
            }
            return t;
        }

        var tut1 = await EnsureTutor("Carolina", "Gómez", "caro.gomez@mail.com", "266-222222");
        var tut2 = await EnsureTutor("Diego", "Martínez", "diego.mtz@mail.com", "266-333333");

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

        // === Torneos (fechas relativas) ===
        var hoy = DateTime.Today;
        var futuros = new[]
        {
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

        // Pasado (para probar que el feed lo ignore salvo que siga en curso)
        var pasado = new Torneo {
            Nombre = "Copa Invierno",
            Lugar = "Club Norte",
            FechaInicio = hoy.AddDays(-20).AddHours(9),
            FechaFin = hoy.AddDays(-18).AddHours(18),
            FechaLimiteInscripcion = hoy.AddDays(-25),
            Organizador = "Liga Regional"
        };
        if (!await db.Torneos.AnyAsync(x => x.Nombre == pasado.Nombre && x.FechaInicio == pasado.FechaInicio))
        { db.Torneos.Add(pasado); await db.SaveChangesAsync(); }

        // === Asistencias (últimos días) ===
        async Task EnsureAsistencia(Patinador p, DateTime fecha, bool presente)
        {
            bool exists = await db.Asistencias.AnyAsync(x => x.PatinadorId == p.PatinadorId && x.FechaClase == fecha.Date);
            if (!exists)
            {
                db.Asistencias.Add(new Asistencia { PatinadorId = p.PatinadorId, FechaClase = fecha.Date, Presente = presente });
                await db.SaveChangesAsync();
            }
        }
        await EnsureAsistencia(cami,  hoy.AddDays(-2), true);
        await EnsureAsistencia(cami,  hoy.AddDays(-1), true);
        await EnsureAsistencia(sofi,  hoy.AddDays(-1), false);
        await EnsureAsistencia(lucia, hoy.AddDays(-3), true);

        // === Evaluaciones técnicas ===
        async Task EnsureEval(Patinador p, string elemento, DateTime fecha, int puntaje, string? obs = null)
        {
            bool exists = await db.Evaluaciones.AnyAsync(x =>
                x.PatinadorId == p.PatinadorId && x.Elemento == elemento && x.Fecha == fecha);
            if (!exists)
            {
                db.Evaluaciones.Add(new EvaluacionTecnica {
                    PatinadorId = p.PatinadorId,
                    Elemento = elemento,
                    Fecha = fecha,
                    Puntaje = puntaje,
                    Observaciones = obs
                });
                await db.SaveChangesAsync();
            }
        }
        await EnsureEval(cami,  "Salto Axel",        hoy.AddDays(-7), 4, "Buena ejecución");
        await EnsureEval(sofi,  "Giro Biellmann",    hoy.AddDays(-5), 3, "Mejorar estabilidad");
        await EnsureEval(lucia, "Secuencia de pasos",hoy.AddDays(-3), 5, "Excelente ritmo");

        // === Pagos (pendiente / pagado) ===
        async Task EnsurePago(Patinador p, string concepto, decimal monto, DateTime venc, string estado, DateTime? fechaPago = null)
        {
            bool exists = await db.Pagos.AnyAsync(x =>
                x.PatinadorId == p.PatinadorId && x.Concepto == concepto && x.FechaVencimiento == venc);
            if (!exists)
            {
                db.Pagos.Add(new Pago {
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
        await EnsurePago(cami,  "Cuota Septiembre", 15000m, hoy.AddDays(9),  "Pendiente");
        await EnsurePago(sofi,  "Cuota Septiembre", 15000m, hoy.AddDays(9),  "Pendiente");
        await EnsurePago(lucia, "Cuota Agosto",     15000m, hoy.AddDays(-20), "Pagado", hoy.AddDays(-15));
    }
}
