using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models;

namespace scooter_rental.Core.Services;

public class ServiceResult
{
    public ServiceResult(bool success)
    {
        Success = success;
        Errors = new List<string>();
    }

    public bool Success { get; }
    public IEntity Entity { get; private set; }

    public IList<string> Errors { get; }

    public string FormattedErrors => string.Join(", ", Errors);

    public ServiceResult SetEntity(IEntity entity)
    {
        Entity = entity;
        return this;
    }

    public ServiceResult AddError(string error)
    {
        Errors.Add(error);
        return this;
    }
}