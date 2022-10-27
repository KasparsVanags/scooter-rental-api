using scooter_rental.Core.Models;

namespace scooter_rental.Core.Interfaces;

public interface IScooter : IEntity
{
    decimal PricePerMinute { get; set; }
    bool IsRented { get; set; }
}