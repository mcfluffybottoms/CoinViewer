using CoinViewer.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Coin> Coins { get; set; } = null!;
    public DbSet<CoinHistoryEntry> CoinHistory { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(u => u.Id).ValueGeneratedOnAdd();
            entity.Property(c => c.Username).IsRequired();
            entity.Property(c => c.Password).IsRequired();
        });
        
        modelBuilder.Entity<Coin>(entity =>
        {
            entity.HasKey(c => c.Symbol);
            entity.Property(c => c.Symbol).IsRequired();
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.Price).IsRequired();
            entity.Property(c => c.LastUpdated).IsRequired();
        });

        modelBuilder.Entity<CoinHistoryEntry>(entity =>
        {
            entity.HasKey(c => new { c.Symbol, c.Timestamp });
            entity.Property(c => c.Symbol).IsRequired();
            entity.Property(c => c.Price).IsRequired();
            entity.Property(c => c.Timestamp).IsRequired();
        });
    }
}