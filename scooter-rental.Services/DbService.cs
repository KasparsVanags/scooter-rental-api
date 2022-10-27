using Microsoft.EntityFrameworkCore;
using scooter_rental.Core.Models;
using scooter_rental.Core.Services;
using scooter_rental.Data;

namespace scooter_rental.Services;

public class DbService : IDbService
{
    protected IScooterRentalDbContext _context;

    public DbService(IScooterRentalDbContext context)
    {
        _context = context;
    }

    public ServiceResult Create<T>(T entity) where T : Entity
    {
        _context.Set<T>().Add(entity);
        _context.SaveChanges();
        return new ServiceResult(true).SetEntity(entity);
    }

    public ServiceResult Delete<T>(T entity) where T : Entity
    {
        _context.Set<T>().Remove(entity);
        _context.SaveChanges();
        return new ServiceResult(true);
    }

    public ServiceResult Update<T>(T entity) where T : Entity
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.SaveChanges();
        return new ServiceResult(true).SetEntity(entity);
    }

    public List<T> GetAll<T>() where T : Entity
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById<T>(string id) where T : Entity
    {
        return _context.Set<T>().SingleOrDefault(e => e.Id == id);
    }

    public IQueryable<T> Query<T>() where T : Entity
    {
        return _context.Set<T>().AsQueryable();
    }
}