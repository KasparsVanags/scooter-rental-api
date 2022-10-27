using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models.ScooterValidators;

namespace scooter_rental.Core.Tests;

public class ScooterIdValidatorTest
{
    private readonly Mock<IScooter> _scooterMock;
    private readonly ScooterIdValidator _scooterIdValidator;

    public ScooterIdValidatorTest()
    {
        _scooterMock = new Mock<IScooter>();
        _scooterIdValidator = new ScooterIdValidator();
    }
    
    [Fact]
    public void IsValid_ValidId_ReturnsTrue()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.Id).Returns("abc");
        
        //Assert
        _scooterIdValidator.IsValid(_scooterMock.Object).Should().Be(true);
    }
    
    [Fact]
    public void IsValid_EmptyId_ReturnsFalse()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.Id).Returns("");
        
        //Assert
        _scooterIdValidator.IsValid(_scooterMock.Object).Should().Be(false);
    }
    
    [Fact]
    public void IsValid_NullId_ReturnsFalse()
    {
        //Arrange
        _scooterMock.SetupGet(x => x.PricePerMinute).Returns(null);
        
        //Assert
        _scooterIdValidator.IsValid(_scooterMock.Object).Should().Be(false);
    }
}