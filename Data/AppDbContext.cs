using Microsoft.EntityFrameworkCore;
using PagosWhatsappAPI.Models;

namespace PagosWhatsappAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<PagoRecibido> PagosRecibidos => Set<PagoRecibido>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Cliente>(e =>
        {
            e.ToTable("Clientes");
            e.HasIndex(x => x.NroDocumento).IsUnique();
        });

        m.Entity<PagoRecibido>(e =>
        {
            e.ToTable("PagosRecibidos");
            e.Property(x => x.Monto).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Cliente)
             .WithMany(c => c.Pagos)
             .HasForeignKey(x => x.ClienteId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        m.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasIndex(x => x.Username).IsUnique();
        });
    }
}