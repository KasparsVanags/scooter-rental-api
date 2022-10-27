namespace scooter_rental.Core.Interfaces;

public interface IScooterValidator
{
    bool IsValid(IScooter scooter);
}