using scooter_rental.Core.Models;

namespace scooter_rental.Core.Services;

public interface IEntityService<T> where T : Entity
{
    ServiceResult Create(T entity);
    ServiceResult Delete(T entity);
    ServiceResult Update(T entity);
    List<T> GetAll();
    T? GetById(string id);
    IQueryable<T> Query();
}