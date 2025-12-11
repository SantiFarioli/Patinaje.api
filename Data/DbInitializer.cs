using Microsoft.EntityFrameworkCore;
using Patinaje.API.Data;
using Patinaje.API.Models;

namespace Patinaje.API.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppPatinContext db)
    {
        // 1. Migraciones
        await db.Database.MigrateAsync();

        // ========================== CLUBES ==========================
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

        // ========================== PROFESORES ==========================
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
        await db.SaveChangesAsync();

        // ========================== PATINADORAS (7) ==========================
        async Task<Patinador> CreatePatinador(string nom, string ape, string dni, string dom, DateTime fnac, string cat, int seed)
        {
            var p = await db.Patinadores.FirstOrDefaultAsync(x => x.Dni == dni);
            if (p is null)
            {
                p = new Patinador
                {
                    Nombre = nom,
                    Apellido = ape,
                    Dni = dni,
                    Domicilio = dom,
                    FechaNacimiento = fnac,
                    Categoria = cat,
                    Activo = true,
                    FichaMedica = "Apta",
                    AsisteGimnasio = true,
                    AsisteNutricionista = false,
                    AsistePsicologo = false,
                    ProfesorId = profDemo.ProfesorId,
                    ClubId = club1.ClubId,
                    FotoUrl = $"https://picsum.photos/seed/{seed}/400"
                };
                db.Patinadores.Add(p);
                await db.SaveChangesAsync();
            }
            return p;
        }

        var cami = await CreatePatinador("Camila", "Gómez", "45322111", "Barrio Sur 101", new DateTime(2012, 6, 10), "B Libre", 101);
        var sofi = await CreatePatinador("Sofía", "Martínez", "46123999", "Calle Mitre 202", new DateTime(2011, 4, 25), "C Escuela", 102);
        var lucia = await CreatePatinador("Lucía", "Pérez", "47222123", "Av. San Martín 789", new DateTime(2010, 9, 5), "A Libre", 103);
        var lola = await CreatePatinador("Lola", "Rivas", "48999123", "Los Álamos 333", new DateTime(2013, 1, 18), "B Escuela", 104);
        var valen = await CreatePatinador("Valentina", "Soria", "49111222", "Barrio Norte 55", new DateTime(2014, 3, 12), "C Libre", 105);
        var marti = await CreatePatinador("Martina", "López", "50222333", "Calle 25 de Mayo 900", new DateTime(2012, 11, 30), "A Escuela", 106);
        var juli = await CreatePatinador("Julieta", "Fernández", "51333444", "Ruta 3 Km 5", new DateTime(2013, 7, 7), "B Libre", 107);

        // ========================== TUTORES ==========================
        async Task<Tutor> EnsureTutor(string nom, string ape, string dni, string email, string tel, string relacion)
        {
            var t = await db.Tutores.FirstOrDefaultAsync(x => x.Email == email);
            if (t is null)
            {
                t = new Tutor
                {
                    Nombre = nom, Apellido = ape, Dni = dni, Domicilio = "Domicilio Tutor",
                    Email = email, Telefono = tel, Relacion = relacion
                };
                db.Tutores.Add(t);
                await db.SaveChangesAsync();
            }
            return t;
        }

        // Creamos tutores (Caro es mamá de Cami y Lucía, para probar hermanos)
        var tCaro = await EnsureTutor("Carolina", "Gómez", "30222333", "caro.gomez@mail.com", "266-222222", "Madre");
        var tDiego = await EnsureTutor("Diego", "Martínez", "30111999", "diego.mtz@mail.com", "266-333333", "Padre");
        var tLaura = await EnsureTutor("Laura", "Rivas", "31444555", "laura.rivas@mail.com", "266-444444", "Madre");
        var tPablo = await EnsureTutor("Pablo", "Soria", "32555666", "pablo.soria@mail.com", "266-555555", "Padre");
        var tAna = await EnsureTutor("Ana", "López", "33666777", "ana.lopez@mail.com", "266-666666", "Madre");
        var tJorge = await EnsureTutor("Jorge", "Fernández", "34777888", "jorge.fer@mail.com", "266-777777", "Padre");

        // ========================== VINCULOS (Tutor-Patinador) ==========================
        async Task Link(Tutor t, Patinador p)
        {
            if (!await db.TutoresPatinadores.AnyAsync(x => x.TutorId == t.TutorId && x.PatinadorId == p.PatinadorId))
            {
                db.TutoresPatinadores.Add(new TutorPatinador { TutorId = t.TutorId, PatinadorId = p.PatinadorId });
                await db.SaveChangesAsync();
            }
        }

        await Link(tCaro, cami);  // Caro -> Cami
        await Link(tCaro, lucia); // Caro -> Lucía (Hermanas)
        await Link(tDiego, sofi);
        await Link(tLaura, lola);
        await Link(tPablo, valen);
        await Link(tAna, marti);
        await Link(tJorge, juli);

        // ========================== TORNEOS (7) ==========================
        if (!db.Torneos.Any())
        {
            var today = DateTime.Today;
            var torneos = new[]
            {
                new Torneo { Nombre = "Copa Invierno 2024", Lugar = "Club Norte", FechaInicio = today.AddMonths(-3), FechaFin = today.AddMonths(-3).AddDays(2), FechaLimiteInscripcion = today.AddMonths(-3).AddDays(-10), Organizador = "Liga Regional" },
                new Torneo { Nombre = "Regional Cuyo", Lugar = "Polideportivo", FechaInicio = today.AddMonths(-1), FechaFin = today.AddMonths(-1).AddDays(3), FechaLimiteInscripcion = today.AddMonths(-1).AddDays(-15), Organizador = "Federación" },
                new Torneo { Nombre = "Torneo Apertura 2025", Lugar = "Club Unión", FechaInicio = today.AddDays(15), FechaFin = today.AddDays(17), FechaLimiteInscripcion = today.AddDays(5), Organizador = "Federación San Luis" },
                new Torneo { Nombre = "Selectivo Provincial", Lugar = "Pista Central", FechaInicio = today.AddMonths(1), FechaFin = today.AddMonths(1).AddDays(2), FechaLimiteInscripcion = today.AddDays(20), Organizador = "Asociación" },
                new Torneo { Nombre = "Nacional B", Lugar = "Mar del Plata", FechaInicio = today.AddMonths(3), FechaFin = today.AddMonths(3).AddDays(5), FechaLimiteInscripcion = today.AddMonths(2), Organizador = "Confederación Argentina" },
                new Torneo { Nombre = "Copa Amistad", Lugar = "Villa Mercedes", FechaInicio = today.AddMonths(4), FechaFin = today.AddMonths(4).AddDays(2), FechaLimiteInscripcion = today.AddMonths(3), Organizador = "Club Mercedes" },
                new Torneo { Nombre = "Clausura 2025", Lugar = "San Luis", FechaInicio = today.AddMonths(6), FechaFin = today.AddMonths(6).AddDays(3), FechaLimiteInscripcion = today.AddMonths(5), Organizador = "Federación" }
            };
            db.Torneos.AddRange(torneos);
            await db.SaveChangesAsync();
        }

        // ========================== PAGOS ==========================
        if (!db.Pagos.Any())
        {
            var now = DateTime.Today;
            db.Pagos.Add(new Pago { PatinadorId = cami.PatinadorId, Concepto = "Cuota Agosto", Monto = 15000, Estado = "Pagado", FechaVencimiento = now.AddMonths(-1), FechaPago = now.AddMonths(-1).AddDays(-2) });
            db.Pagos.Add(new Pago { PatinadorId = lucia.PatinadorId, Concepto = "Inscripción Torneo", Monto = 25000, Estado = "Pagado", FechaVencimiento = now.AddDays(-10), FechaPago = now.AddDays(-12) });
            db.Pagos.Add(new Pago { PatinadorId = cami.PatinadorId, Concepto = "Cuota Septiembre", Monto = 18000, Estado = "Pendiente", FechaVencimiento = now.AddDays(5) });
            db.Pagos.Add(new Pago { PatinadorId = sofi.PatinadorId, Concepto = "Cuota Septiembre", Monto = 18000, Estado = "Pendiente", FechaVencimiento = now.AddDays(5) });
            db.Pagos.Add(new Pago { PatinadorId = lola.PatinadorId, Concepto = "Malla Competición", Monto = 45000, Estado = "Pendiente", FechaVencimiento = now.AddDays(10) });
            await db.SaveChangesAsync();
        }

        // ========================== ASISTENCIAS ==========================
        if (!db.Asistencias.Any())
        {
            var dias = new[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-3), DateTime.Today.AddDays(-5) };
            foreach (var d in dias)
            {
                // Asistencia aleatoria
                db.Asistencias.Add(new Asistencia { PatinadorId = cami.PatinadorId, FechaClase = d, Presente = true });
                db.Asistencias.Add(new Asistencia { PatinadorId = sofi.PatinadorId, FechaClase = d, Presente = true });
                db.Asistencias.Add(new Asistencia { PatinadorId = lucia.PatinadorId, FechaClase = d, Presente = false });
                db.Asistencias.Add(new Asistencia { PatinadorId = lola.PatinadorId, FechaClase = d, Presente = true });
                db.Asistencias.Add(new Asistencia { PatinadorId = valen.PatinadorId, FechaClase = d, Presente = true });
            }
            await db.SaveChangesAsync();
        }
    }
}