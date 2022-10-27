using scooter_rental.Core.Models;
using scooter_rental.Core.Services;
using scooter_rental.Data;

namespace scooter_rental.Services;

public class EntityService<T> : DbService, IEntityService<T> where T : Entity
{
    public EntityService(IScooterRentalDbContext context) : base(context)
    {
    }

    public ServiceResult Create(T entity)
    {
        return Create<T>(entity);
    }

    public ServiceResult Delete(T entity)
    {
        return Delete<T>(entity);
    }

    public ServiceResult Update(T entity)
    {
        return Update<T>(entity);
    }

    public List<T> GetAll()
    {
        return GetAll<T>();
    }

    public T? GetById(string id)
    {
        return GetById<T>(id);
    }

    public IQueryable<T> Query()
    {
        return Query<T>();
    }
}