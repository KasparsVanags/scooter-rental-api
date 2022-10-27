using scooter_rental.Core.Models;

namespace scooter_rental.Core.Services;

public interface IDbService
{
    ServiceResult Create<T>(T entity) where T : Entity;
    ServiceResult Delete<T>(T entity) where T : Entity;
    ServiceResult Update<T>(T entity) where T : Entity;
    List<T> GetAll<T>() where T : Entity;
    T? GetById<T>(string id) where T : Entity;
    IQueryable<T> Query<T>() where T : Entity;
}