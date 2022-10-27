using scooter_rental.Core.Services;

namespace scooter_rental.Services;

public interface IRentalService
{
    ServiceResult StartRent(string id, DateTime time);
    ServiceResult EndRent(string id, DateTime time);
    decimal GetIncome(int? year, bool includeIncompleteRentals, DateTime? currentTime);
}