using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using scooter_rental.Core.Models;

namespace scooter_rental.Data;

public interface IScooterRentalDbContext
{
    DbSet<Scooter> Scooters { get; set; }
    DbSet<RentalPeriod> RentalPeriods { get; set; }
    DbSet<T> Set<T>() where T : class;
    EntityEntry<T> Entry<T>(T entity) where T : class;
    int SaveChanges();
    Task<int> SaveChangesAsync();
}