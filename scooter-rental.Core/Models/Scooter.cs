using scooter_rental.Core.Interfaces;

namespace scooter_rental.Core.Models;

public class Scooter : Entity, IScooter
{
    public decimal PricePerMinute { get; set; }
    public bool IsRented { get; set; }
}