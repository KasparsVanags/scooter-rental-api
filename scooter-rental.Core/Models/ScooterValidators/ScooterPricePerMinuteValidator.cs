using scooter_rental.Core.Interfaces;

namespace scooter_rental.Core.Models.ScooterValidators;

public class ScooterPricePerMinuteValidator : IScooterValidator
{
    public bool IsValid(IScooter scooter)
    {
        return scooter.PricePerMinute > 0;
    }
}