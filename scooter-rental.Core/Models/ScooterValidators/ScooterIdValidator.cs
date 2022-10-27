using scooter_rental.Core.Interfaces;

namespace scooter_rental.Core.Models.ScooterValidators;

public class ScooterIdValidator : IScooterValidator
{
    public bool IsValid(IScooter scooter)
    {
        return !string.IsNullOrEmpty(scooter.Id);
    }
}