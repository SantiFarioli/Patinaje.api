using Microsoft.EntityFrameworkCore;
using Patinaje.API.Models;

public class AppPatinContext : DbContext
{
    public AppPatinContext(DbContextOptions<AppPatinContext> options) : base(options) {}

    public DbSet<Profesor> Profesores => Set<Profesor>();
    public DbSet<Patinador> Patinadores => Set<Patinador>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<TutorPatinador> TutoresPatinadores => Set<TutorPatinador>();
    public DbSet<Torneo> Torneos => Set<Torneo>();
    public DbSet<InscripcionTorneo> Inscripciones => Set<InscripcionTorneo>();
    public DbSet<EvaluacionTecnica> Evaluaciones => Set<EvaluacionTecnica>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<Club> Clubes => Set<Club>();



    protected override void OnModelCreating(ModelBuilder mb)
  {
    // M:N Patinador <-> Tutor
    mb.Entity<TutorPatinador>().HasKey(x => new { x.TutorId, x.PatinadorId });

    mb.Entity<TutorPatinador>()
      .HasOne(x => x.Tutor)
      .WithMany(t => t.Patinadores)
      .HasForeignKey(x => x.TutorId);

    mb.Entity<TutorPatinador>()
      .HasOne(x => x.Patinador)
      .WithMany(p => p.Tutores)
      .HasForeignKey(x => x.PatinadorId);

    // Reglas simples
    mb.Entity<Profesor>().Property(p => p.Email).IsRequired().HasMaxLength(120);
    mb.Entity<Patinador>().Property(p => p.Categoria).IsRequired().HasMaxLength(40);
  }
}
