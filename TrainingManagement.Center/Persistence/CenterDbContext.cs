using Microsoft.EntityFrameworkCore;
using TrainingManagement.Center.Models;

namespace TrainingManagement.Center.Persistence;

public class CenterDbContext : DbContext
{
    public DbSet<TrainingCenter> TrainingCenters => Set<TrainingCenter>();
    public CenterDbContext(DbContextOptions<CenterDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("center");
        modelBuilder.Entity<TrainingCenter>(opt =>
        {
            opt.HasKey(x => x.Id);
            opt.Property(x => x.Name)
                .HasMaxLength(50)
                .IsRequired();
            opt.Property(x => x.Slug)
                .HasMaxLength(100)
                .IsRequired();

            opt.Property(x => x.Description)
                .IsRequired();
            opt.Property(x => x.Address)
                .HasMaxLength(150)
                .IsRequired();
            opt.Property(x => x.Municipality)
                .HasMaxLength(50)
                .IsRequired();
            opt.Property(x => x.ContactNo)
                .HasMaxLength(50)
                .IsRequired();
        });
    }
}
