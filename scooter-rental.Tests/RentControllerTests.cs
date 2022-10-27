using Microsoft.AspNetCore.Mvc;
using scooter_rental_api.Controllers;
using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models;
using scooter_rental.Core.Services;
using scooter_rental.Services;

namespace scooter_rental.Tests;

public class RentControllerTests
{
    private readonly RentController _rentController;
    private readonly Mock<IRentalService> _rentalServiceMock;
    private readonly Mock<Scooter> _scooterMock;
    private readonly DateTime _testTime;
    private readonly Mock<IEntity> _entityMock;

    public RentControllerTests()
    {
        _rentalServiceMock = new Mock<IRentalService>();
        _scooterMock = new Mock<Scooter>();
        _testTime = new DateTime(2022, 2, 2);
        _rentController = new RentController(_rentalServiceMock.Object);
        _entityMock = new Mock<IEntity>();
    }

    [Fact]
    public void StartRent_RentStarted_ReturnsOk()
    {
        //Arrange
        _entityMock.SetupGet(x => x.Id).Returns("abc");
        var successResult = new ServiceResult(true).SetEntity(_entityMock.Object);
       
        _rentalServiceMock.Setup(x => x.StartRent("abc", _testTime)).Returns(successResult);
        
        //Act
        var result = _rentController.StartRent("abc", _testTime) as OkObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(successResult.Entity);
        _rentalServiceMock.Verify(x => x.StartRent("abc", _testTime), Times.Once);
    }
    
    [Fact]
    public void StartRent_RentNotStarted_ReturnsBadRequest()
    {
        //Arrange
        _entityMock.SetupGet(x => x.Id).Returns("abc");
        var successResult = new ServiceResult(false).SetEntity(_entityMock.Object).AddError("error");
        _rentalServiceMock.Setup(x => x.StartRent("abc", _testTime)).Returns(successResult);
        
        //Act
        var result = _rentController.StartRent("abc", _testTime) as BadRequestObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeEquivalentTo(successResult.FormattedErrors);
        _rentalServiceMock.Verify(x => x.StartRent("abc", _testTime), Times.Once);
    }
    
    [Fact]
    public void EndRent_RentEnded_ReturnsOk()
    {
        //Arrange
        _entityMock.SetupGet(x => x.Id).Returns("abc");
        var successResult = new ServiceResult(true).SetEntity(_entityMock.Object);
       
        _rentalServiceMock.Setup(x => x.EndRent("abc", _testTime)).Returns(successResult);
        
        //Act
        var result = _rentController.EndRent("abc", _testTime) as OkObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(successResult.Entity);
        _rentalServiceMock.Verify(x => x.EndRent("abc", _testTime), Times.Once);
    }
    
    [Fact]
    public void StartRent_RentNotEnded_ReturnsBadRequest()
    {
        //Arrange
        _entityMock.SetupGet(x => x.Id).Returns("abc");
        var successResult = new ServiceResult(false).SetEntity(_entityMock.Object).AddError("error");
        _rentalServiceMock.Setup(x => x.EndRent("abc", _testTime)).Returns(successResult);
        
        //Act
        var result = _rentController.EndRent("abc", _testTime) as BadRequestObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().BeEquivalentTo(successResult.FormattedErrors);
        _rentalServiceMock.Verify(x => x.EndRent("abc", _testTime), Times.Once);
    }

    [Fact]
    public void GetIncome_ValidInfo_CallsRentalService()
    {
        //Arrange
        _rentalServiceMock.Setup(x => x.GetIncome(_testTime.Year, true, _testTime)).Returns(1m);
        
        //Act
        var result = _rentController.GetIncome(_testTime.Year, true, _testTime) as OkObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be(1m);
    }
    
    [Fact]
    public void GetIncome_IncompleteRentalsNoCurrentTime_ReturnsBadRequest()
    {
        //Act
        var result = _rentController.GetIncome(_testTime.Year, true, null) as BadRequestObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().Be("Current time cannot be null when requesting income from incomplete rentals");
    }
    
    [Fact]
    public void GetIncome_CurrentYearBeforeReportYear_ReturnsBadRequest()
    {
        //Act
        var result = _rentController
            .GetIncome(_testTime.Year, true, _testTime - TimeSpan.FromDays(500)) as BadRequestObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(400);
        result.Value.Should().Be("Current year cannot be before year of report");
    }
}