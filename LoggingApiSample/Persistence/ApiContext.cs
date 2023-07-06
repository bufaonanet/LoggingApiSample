using LoggingApiSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoggingApiSample.Persistence;

public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Indicador> Indicadores { get; set; } = null!;
    public DbSet<BolsaValores> BolsasValores { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Indicador>()
               .HasKey(i => i.Sigla);

        modelBuilder.Entity<BolsaValores>()
            .HasKey(b => b.Sigla);
    }
}