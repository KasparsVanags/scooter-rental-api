using Microsoft.EntityFrameworkCore;
using scooter_rental.Core.Models;

namespace scooter_rental.Data;

public class ScooterRentalDbContext : DbContext, IScooterRentalDbContext
{
    public ScooterRentalDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Scooter> Scooters { get; set; }
    public DbSet<RentalPeriod> RentalPeriods { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RentalPeriod>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
    }
}