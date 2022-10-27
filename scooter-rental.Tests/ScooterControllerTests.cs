using Microsoft.AspNetCore.Mvc;
using scooter_rental_api.Controllers;
using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models;
using scooter_rental.Core.Services;

namespace scooter_rental.Tests;

public class ScooterControllerTests
{
    private readonly ScooterController _scooterController;
    private readonly Mock<Scooter> _scooterMock;
    private readonly Mock<IScooterValidator> _validatorMock;
    private readonly Mock<IEntityService<Scooter>> _entityServiceMock;

    public ScooterControllerTests()
    {
        _scooterMock = new Mock<Scooter>();
        _validatorMock = new Mock<IScooterValidator>();
        _entityServiceMock = new Mock<IEntityService<Scooter>>();
        _scooterController = new ScooterController(_entityServiceMock.Object, new []{_validatorMock.Object});
    }

    [Fact]
    public void AddScooter_ValidScooter_AddsScooter()
    {
        //Arrange
        _validatorMock.Setup(x => x.IsValid(_scooterMock.Object)).Returns(true);
        _entityServiceMock.Setup(x => x.Create(_scooterMock.Object));
        
        //Act
        var result = _scooterController.AddScooter(_scooterMock.Object) as CreatedResult;

        //Assert
        result.Value.Should().Be(_scooterMock.Object);
        result.StatusCode.Should().Be(201);
        _entityServiceMock.Verify(x => x.Create(_scooterMock.Object), Times.Once);
    }
    
    [Fact]
    public void AddScooter_InValidScooter_ReturnsBadRequest()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.Create(_scooterMock.Object));
        _validatorMock.Setup(x => x.IsValid(_scooterMock.Object)).Returns(false);
        
        //Act
        var result = _scooterController.AddScooter(_scooterMock.Object) as BadRequestResult;

        //Assert
        result.StatusCode.Should().Be(400);
        _entityServiceMock.Verify(x => x.Create(_scooterMock.Object), Times.Never);
    }
    
    [Fact]
    public void AddScooter_Duplicate_ReturnsConflict()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.Create(_scooterMock.Object));
        _validatorMock.Setup(x => x.IsValid(_scooterMock.Object)).Returns(true);
        _entityServiceMock.Setup(x => x.GetById(_scooterMock.Object.Id)).Returns(_scooterMock.Object);
        
        //Act
        _scooterController.AddScooter(_scooterMock.Object);
        var result = _scooterController.AddScooter(_scooterMock.Object) as ConflictObjectResult;

        //Assert
        result.StatusCode.Should().Be(409);
        _entityServiceMock.Verify(x => x.Create(_scooterMock.Object), Times.Never);
    }

    [Fact]
    public void DeleteScooter_ScooterExists_DeletesScooter()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.Delete(_scooterMock.Object));
        _entityServiceMock.Setup(x => x.GetById("abc")).Returns(_scooterMock.Object);
        
        //Act
        var result = _scooterController.DeleteScooter("abc") as OkResult;
        
        //Assert
        result.StatusCode.Should().Be(200);
        _entityServiceMock.Verify(x => x.Delete(_scooterMock.Object), Times.Once);
    }
    
    [Fact]
    public void DeleteScooter_ScooterDoesntExist_ReturnsNotFound()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.Delete(_scooterMock.Object));
        _entityServiceMock.Setup(x => x.GetById("abc")).Returns((Scooter)null);
        
        //Act
        var result = _scooterController.DeleteScooter("abc") as NotFoundResult;
        
        //Assert
        result.StatusCode.Should().Be(404);
        _entityServiceMock.Verify(x => x.Delete(_scooterMock.Object), Times.Never);
    }

    [Fact]
    public void GetScooter_ScooterExists_ReturnsScooter()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.GetById("abc")).Returns(_scooterMock.Object);
        
        //Act
        var result = _scooterController.GetScooter("abc") as OkObjectResult;

        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().Be(_scooterMock.Object);
    }
    
    [Fact]
    public void GetScooter_ScooterDoesNotExist_ReturnsNotFound()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.GetById("abc")).Returns((Scooter)null);
        
        //Act
        var result = _scooterController.GetScooter("abc") as NotFoundObjectResult;

        //Assert
        result.StatusCode.Should().Be(404);
        result.Value.Should().Be($"Scooter id abc does not exist");
    }

    [Fact]
    public void GetAllScooters_ScootersExist_ReturnsAllScooters()
    {
        //Arrange
        var scooterList = new List<Scooter>
        {
            _scooterMock.Object
        };
        _entityServiceMock.Setup(x => x.GetAll()).Returns(scooterList);
        
        //Act
        var result = _scooterController.GetAllScooters() as OkObjectResult;
        
        //Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(scooterList);
    }
    
    [Fact]
    public void GetAllScooters_NoScooters_ReturnsNoContent()
    {
        //Arrange
        _entityServiceMock.Setup(x => x.GetAll()).Returns(new List<Scooter>());
        
        //Act
        var result = _scooterController.GetAllScooters() as NoContentResult;
        
        //Assert
        result.StatusCode.Should().Be(204);
    }
}