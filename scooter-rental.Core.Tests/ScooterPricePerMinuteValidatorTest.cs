using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models.ScooterValidators;

namespace scooter_rental.Core.Tests;

public class ScooterPricePerMinuteValidatorTest
{
    private readonly Mock<IScooter> _scooterMock;
    private readonly ScooterPricePerMinuteValidator _scooterPricePerMinuteValidator;

    public ScooterPricePerMinuteValidatorTest()
    {
        _scooterMock = new Mock<IScooter>();
        _scooterPricePerMinuteValidator = new ScooterPricePerMinuteValidator();
    }
    
    [Fact]
    public void IsValid_ValidPrice_ReturnsTrue()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.PricePerMinute).Returns(1);
        
        //Assert
        _scooterPricePerMinuteValidator.IsValid(_scooterMock.Object).Should().Be(true);
    }
    
    [Fact]
    public void IsValid_ZeroPrice_ReturnsFalse()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.PricePerMinute).Returns(0);
        
        //Assert
        _scooterPricePerMinuteValidator.IsValid(_scooterMock.Object).Should().Be(false);
    }
    
    [Fact]
    public void IsValid_NegativePrice_ReturnsFalse()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.PricePerMinute).Returns(-10);
        
        //Assert
        _scooterPricePerMinuteValidator.IsValid(_scooterMock.Object).Should().Be(false);
    }
}