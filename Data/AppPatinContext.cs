using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

namespace Patinaje.API.Data
{
    public class AppPatinContext : DbContext
    {
        public AppPatinContext(DbContextOptions<AppPatinContext> options) : base(options) { }

        // === Tablas principales ===
        public DbSet<Profesor> Profesores => Set<Profesor>();
        public DbSet<Patinador> Patinadores => Set<Patinador>();
        public DbSet<Tutor> Tutores => Set<Tutor>();
        public DbSet<TutorPatinador> TutoresPatinadores => Set<TutorPatinador>();
        public DbSet<Torneo> Torneos => Set<Torneo>();
        public DbSet<InscripcionTorneo> Inscripciones => Set<InscripcionTorneo>();
        public DbSet<EvaluacionTecnica> Evaluaciones => Set<EvaluacionTecnica>();
        public DbSet<EvaluacionTorneo> EvaluacionesTorneos => Set<EvaluacionTorneo>();
        public DbSet<DetalleElemento> DetallesElementos => Set<DetalleElemento>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();
        public DbSet<Club> Clubes => Set<Club>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // === Relación M:N Patinador <-> Tutor ===
            mb.Entity<TutorPatinador>().HasKey(x => new { x.TutorId, x.PatinadorId });

            mb.Entity<TutorPatinador>()
                .HasOne(x => x.Tutor)
                .WithMany(t => t.Patinadores)
                .HasForeignKey(x => x.TutorId);

            mb.Entity<TutorPatinador>()
                .HasOne(x => x.Patinador)
                .WithMany(p => p.Tutores)
                .HasForeignKey(x => x.PatinadorId);

            // === Relación 1:N EvaluacionTorneo -> DetalleElemento ===
            mb.Entity<DetalleElemento>()
                .HasOne(d => d.EvaluacionTorneo)
                .WithMany(e => e.Detalles)
                .HasForeignKey(d => d.EvaluacionTorneoId)
                .OnDelete(DeleteBehavior.Cascade);

            // === Restricciones de longitud obligatorias ===
            mb.Entity<Profesor>().Property(p => p.Email).IsRequired().HasMaxLength(120);
            mb.Entity<Patinador>().Property(p => p.Categoria).IsRequired().HasMaxLength(40);

            // === Nombres de tablas ===
            mb.Entity<Profesor>().ToTable("Profesores");
            mb.Entity<Patinador>().ToTable("Patinadores");
            mb.Entity<Tutor>().ToTable("Tutores");
            mb.Entity<TutorPatinador>().ToTable("TutoresPatinadores");
            mb.Entity<Torneo>().ToTable("Torneos");
            mb.Entity<InscripcionTorneo>().ToTable("Inscripciones");
            mb.Entity<EvaluacionTecnica>().ToTable("EvaluacionesTecnicas");
            mb.Entity<EvaluacionTorneo>().ToTable("EvaluacionesTorneos");
            mb.Entity<DetalleElemento>().ToTable("DetallesElementos");
            mb.Entity<Pago>().ToTable("Pagos");
            mb.Entity<Asistencia>().ToTable("Asistencias");
            mb.Entity<Club>().ToTable("Clubes");
        }
    }
}
