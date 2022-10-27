using scooter_rental.Core.Calculators;
using scooter_rental.Core.Models;
using scooter_rental.Core.Services;
using scooter_rental.Data;

namespace scooter_rental.Services;

public class RentalService : EntityService<RentalPeriod>, IRentalService
{
    public RentalService(IScooterRentalDbContext context) : base(context)
    {
    }

    public ServiceResult StartRent(string id, DateTime time)
    {
        var scooter = _context.Scooters.FirstOrDefault(x => x.Id == id);
        if (scooter == null) return new ServiceResult(false).AddError($"Scooter id {id} not found");

        if (scooter.IsRented) return new ServiceResult(false).AddError($"Scooter id {id} already rented");

        scooter.IsRented = true;
        Update(scooter);

        return Create(new RentalPeriod
        {
            Scooter = scooter,
            StartTime = time,
            PricePerMinute = scooter.PricePerMinute
        });
    }

    public ServiceResult EndRent(string id, DateTime time)
    {
        var scooter = _context.Scooters.FirstOrDefault(x => x.Id == id);
        if (scooter == null) return new ServiceResult(false).AddError($"Scooter id {id} not found");

        if (!scooter.IsRented) return new ServiceResult(false).AddError($"Scooter id {id} not rented");

        var period = _context.RentalPeriods.FirstOrDefault(x => x.Scooter == scooter && x.EndTime == null);
        if (period == null)
            return new ServiceResult(false)
                .AddError("A rented scooter was found but a matching rental period does not exist");

        if (time <= period.StartTime)
            return new ServiceResult(false)
                .AddError($"Rental period was started at {period.StartTime} and cannot end at {time}");

        period.EndTime = time;

        scooter.IsRented = false;
        Update(scooter);

        return Update(period);
    }

    public decimal GetIncome(int? year, bool includeIncompleteRentals, DateTime? currentTime)
    {
        decimal completedRentals;
        if (year == null)
            completedRentals = _context.RentalPeriods.Where(x => x.EndTime != null).ToList()
                .Sum(x => IncomeCalculator.GetIncome(x.StartTime, x.EndTime.Value, x.PricePerMinute));
        else
            completedRentals = _context.RentalPeriods
                .Where(x => x.EndTime != null && x.EndTime.Value.Year == year).ToList()
                .Sum(x => IncomeCalculator.GetIncome(x.StartTime, x.EndTime.Value, x.PricePerMinute));

        if (!includeIncompleteRentals) return completedRentals;

        decimal incompleteRentals;
        if (year == null)
            incompleteRentals = _context.RentalPeriods.Where(x => x.EndTime == null).ToList()
                .Sum(x => IncomeCalculator.GetIncome(x.StartTime, currentTime.Value, x.PricePerMinute));
        else
            incompleteRentals = _context.RentalPeriods
                .Where(x => x.StartTime.Year == year
                            && x.StartTime.Year == currentTime.Value.Year
                            && x.EndTime == null).ToList()
                .Sum(x => IncomeCalculator.GetIncome(x.StartTime, currentTime.Value, x.PricePerMinute));

        return completedRentals + incompleteRentals;
    }
}