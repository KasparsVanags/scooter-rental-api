using scooter_rental.Core.Models;

namespace scooter_rental.Core.Interfaces;

public interface IRentalPeriod : IEntity
{
    Scooter Scooter { get; set; }
    DateTime StartTime { get; set; }
    decimal PricePerMinute { get; set; }
    DateTime? EndTime { get; set; }
}