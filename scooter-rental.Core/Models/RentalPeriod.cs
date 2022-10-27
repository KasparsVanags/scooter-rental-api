using Microsoft.EntityFrameworkCore;
using scooter_rental.Core.Interfaces;

namespace scooter_rental.Core.Models;

public class RentalPeriod : Entity, IRentalPeriod
{
    public Scooter Scooter { get; set; }
    public DateTime StartTime { get; set; }
    public decimal PricePerMinute { get; set; }
    public DateTime? EndTime { get; set; }
}