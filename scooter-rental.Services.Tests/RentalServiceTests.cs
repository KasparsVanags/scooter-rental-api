using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using scooter_rental.Core.Calculators;
using scooter_rental.Core.Models;
using scooter_rental.Data;

namespace scooter_rental.Services.Tests;

public class RentalServiceTests : TestWithSqlite
{
    private readonly RentalService _rentalService;
    private readonly DateTime _testTime = new DateTime(2022, 1, 1, 12, 0, 0);

    public RentalServiceTests()
    {
        _rentalService = new RentalService(DbContext);
    }

    [Fact]
    public async Task DatabaseIsAvailableAndCanBeConnectedTo()
    {
        Assert.True(await DbContext.Database.CanConnectAsync());
    }
    
    [Fact]
    public void StartRent_ScooterExists_StartsRent()
    {
        //Arrange
        var scooter = new Scooter() { Id = "abc", IsRented = false, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.StartRent("abc", _testTime);
        
        //Assert
        DbContext.Scooters.First().IsRented.Should().BeTrue();
        var period = DbContext.RentalPeriods.First();
        period.Scooter.Should().BeEquivalentTo(scooter);
        period.StartTime.Should().Be(_testTime);
        period.EndTime.Should().Be(null);
        period.PricePerMinute.Should().Be(10);
        ((RentalPeriod)result.Entity).Scooter.Should().BeEquivalentTo(scooter);
        ((RentalPeriod)result.Entity).StartTime.Should().Be(_testTime);
        ((RentalPeriod)result.Entity).EndTime.Should().Be(null);
        ((RentalPeriod)result.Entity).PricePerMinute.Should().Be(10);
    }

    [Fact]
    public void StartRent_ScooterDoesNotExist_ReturnsScooterNotFoundError()
    {
        //Act
        var result = _rentalService.StartRent("abc", _testTime);
        
        //Assert
        result.FormattedErrors.Should().Be($"Scooter id abc not found");
    }
    
    [Fact]
    public void StartRent_ScooterAlreadyRented_ReturnsScooterAlreadyRentedError()
    {
        //Arrange
        var scooter = new Scooter() { Id = "abc", IsRented = true, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.StartRent("abc", _testTime);
        
        //Assert
        result.FormattedErrors.Should().Be($"Scooter id abc already rented");
    }
    
    [Fact]
    public void EndRent_ScooterExists_EndsRent()
    {
        //Arrange
        var endTime = _testTime + TimeSpan.FromDays(1);
        var scooter = new Scooter() { Id = "abc", IsRented = true, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.RentalPeriods.Add(new RentalPeriod()
        {
            Id = "def",
            Scooter = scooter,
            StartTime = _testTime,
            PricePerMinute = 10
        });
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.EndRent("abc", endTime);
        
        //Assert
        DbContext.Scooters.First().IsRented.Should().BeFalse();
        var period = DbContext.RentalPeriods.First();
        period.Scooter.Should().BeEquivalentTo(scooter);
        period.StartTime.Should().Be(_testTime);
        period.EndTime.Should().Be(endTime);
        period.PricePerMinute.Should().Be(10);
        ((RentalPeriod)result.Entity).Scooter.Should().BeEquivalentTo(scooter);
        ((RentalPeriod)result.Entity).StartTime.Should().Be(_testTime);
        ((RentalPeriod)result.Entity).EndTime.Should().Be(endTime);
        ((RentalPeriod)result.Entity).PricePerMinute.Should().Be(10);
    }

    [Fact]
    public void EndRent_ScooterDoesNotExist_ReturnsScooterNotFoundError()
    {
        //Act
        var result = _rentalService.EndRent("abc", _testTime);
        
        //Assert
        result.FormattedErrors.Should().Be("Scooter id abc not found");
    }
    
    [Fact]
    public void EndRent_ScooterNotRented_ReturnsScooterNotRentedError()
    {
        //Arrange
        var scooter = new Scooter() { Id = "abc", IsRented = false, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.EndRent("abc", _testTime);
        
        //Assert
        result.FormattedErrors.Should().Be($"Scooter id abc not rented");
    }
    
    [Fact]
    public void EndRent_ScooterRentedButMissingRentPeriod_ReturnsMissingRentPeriodError()
    {
        //Arrange
        var scooter = new Scooter() { Id = "abc", IsRented = true, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.EndRent("abc", _testTime);
        
        //Assert
        result.FormattedErrors.Should().Be("A rented scooter was found but a matching rental period does not exist");
    }
    
    [Fact]
    public void EndRent_EndTimeBeforeStartTime_ReturnsRentTimeError()
    {
        //Arrange
        var invalidEndTime = _testTime - TimeSpan.FromDays(1);
        var scooter = new Scooter() { Id = "abc", IsRented = true, PricePerMinute = 10};
        DbContext.Scooters.Add(scooter);
        DbContext.RentalPeriods.Add(new RentalPeriod()
        {
            Id = "def",
            Scooter = scooter,
            StartTime = _testTime,
            PricePerMinute = 10
        });
        DbContext.SaveChanges();
        
        //Act
        var result = _rentalService.EndRent("abc", invalidEndTime);
        
        //Assert
        result.FormattedErrors.Should().Be($"Rental period was started at {_testTime} and cannot end at {invalidEndTime}");
    }
    
    [Fact]
    public void CalculateIncome_AllCompletedRentals_ReturnsIncome()
    {
        //Arrange
        var date = new DateTime(2000, 1, 1);
        DbContext.RentalPeriods.Add(new RentalPeriod()
        {
            Id = "1",
            StartTime = date,
            PricePerMinute = 10
        });
        DbContext.SaveChanges();
        DbContext.RentalPeriods.First().EndTime = date.AddMinutes(10);
        DbContext.SaveChanges();
        
        //Assert
        _rentalService.GetIncome(null, false, _testTime).Should().Be(20);
    }
    
    [Fact]
    public void CalculateIncome_AllRentals_ReturnsIncome()
    {
        //Arrange
        var date = _testTime - TimeSpan.FromMinutes(10);
        DbContext.RentalPeriods.Add(new RentalPeriod()
        {
            Id = "1",
            StartTime = date,
            PricePerMinute = 1
        });
        DbContext.SaveChanges();
        
        //Assert
        _rentalService.GetIncome(null, true, _testTime).Should().Be(10);
    }

    [Fact]
    public void CalculateIncome_AllCompletedRentals_IsCorrect()
    {
        //Arrange
        DbContext.RentalPeriods.AddRange(CompletedRentals.Select(x => (RentalPeriod)x[0]));
        DbContext.SaveChanges();
        var sum = CompletedRentals.Sum(x => (decimal)x[1]);
        
        //Assert
        _rentalService.GetIncome(null, false, _testTime).Should().Be(sum);
    }
    
    [Fact]
    public void CalculateIncome_AllRentalsIncludingIncompleteRentals_IsCorrect()
    {
        //Arrange
        DbContext.RentalPeriods.AddRange(CompletedRentals.Select(x => (RentalPeriod)x[0]));
        DbContext.SaveChanges();
        var incompleteRentals = CompletedRentals.Select(x => (RentalPeriod)x[0]).ToList();
        incompleteRentals.ForEach(x => x.EndTime = null);
        incompleteRentals.ForEach(x => x.Scooter.Id += "1");
        DbContext.RentalPeriods.AddRange(incompleteRentals);
        DbContext.SaveChanges();
        var sum = CompletedRentals.Sum(x => (decimal)x[1]) 
                  + incompleteRentals.Sum(x => IncomeCalculator.GetIncome(x.StartTime, _testTime, x.PricePerMinute));
        
        //Assert
        _rentalService.GetIncome(null, true, _testTime).Should().Be(sum);
    }

    [Theory]
    [InlineData(2021)]
    [InlineData(2020)]
    [InlineData(2019)]
    public void CalculateIncome_AllCompletedRentalsForSpecificYear_IsCorrect(int year)
    {
        //Arrange
        DbContext.RentalPeriods.AddRange(CompletedRentals.Select(x => (RentalPeriod)x[0]));
        DbContext.SaveChanges();
        var incompleteRentals = CompletedRentals.Select(x => (RentalPeriod)x[0]).ToList();
        incompleteRentals.ForEach(x => x.EndTime = null);
        incompleteRentals.ForEach(x => x.Scooter.Id += "1");
        DbContext.RentalPeriods.AddRange(incompleteRentals);
        DbContext.SaveChanges();
        var sum = DbContext.RentalPeriods.Where(x => x.EndTime != null && x.EndTime.Value.Year == year).ToList()
            .Sum(x => IncomeCalculator.GetIncome(x.StartTime, x.EndTime.Value, x.PricePerMinute));
        
        //Assert
        _rentalService.GetIncome(year, false, _testTime).Should().Be(sum);
    }
    
    [Theory]
    [InlineData(2021)]
    [InlineData(2020)]
    [InlineData(2019)]
    public void CalculateIncome_AllRentalsForSpecificYearWhenIncompleteRentalsWillEndInFuture_IsCorrect(int year)
    {
        //Arrange
        DbContext.RentalPeriods.AddRange(CompletedRentals.Select(x => (RentalPeriod)x[0]));
        DbContext.SaveChanges();
        var incompleteRentals = CompletedRentals.Select(x => (RentalPeriod)x[0]).ToList();
        incompleteRentals.ForEach(x => x.EndTime = null);
        incompleteRentals.ForEach(x => x.Scooter.Id += "1");
        DbContext.RentalPeriods.AddRange(incompleteRentals);
        DbContext.SaveChanges();
        var sum = DbContext.RentalPeriods.Where(x => x.EndTime != null && x.EndTime.Value.Year == year).ToList()
            .Sum(x => IncomeCalculator.GetIncome(x.StartTime, x.EndTime.Value, x.PricePerMinute));
        
        //Assert
        _rentalService.GetIncome(year, true, _testTime).Should().Be(sum);
    }
    
    [Fact]
    public void CalculateIncome_AllRentalsForSpecificYearWhenIncompleteRentalsWillEndThisYear_IsCorrect()
    {
        //Arrange
        DbContext.RentalPeriods.AddRange(CompletedRentals.Select(x => (RentalPeriod)x[0]));
        DbContext.SaveChanges();
        var incompleteRentals = CompletedRentals.Select(x => (RentalPeriod)x[0]).ToList();
        incompleteRentals.ForEach(x => x.EndTime = null);
        incompleteRentals.ForEach(x => x.Scooter.Id += "1");
        DbContext.RentalPeriods.AddRange(incompleteRentals);
        DbContext.SaveChanges();
        var sum = DbContext.RentalPeriods.Where(x => x.EndTime != null && x.EndTime.Value.Year == _testTime.Year).ToList()
                      .Sum(x => IncomeCalculator.GetIncome(x.StartTime, _testTime, x.PricePerMinute))
                  + incompleteRentals.Where(x => x.StartTime.Year == _testTime.Year).ToList()
                      .Sum(x => IncomeCalculator.GetIncome(x.StartTime, _testTime, x.PricePerMinute));
        //Assert
        _rentalService.GetIncome(_testTime.Year, true, _testTime).Should().Be(sum);
    }
    
    private static IEnumerable<object[]> CompletedRentals =>
        new List<object[]>
        {
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "1"},
                StartTime = new DateTime(2020, 1, 1, 12, 0, 0),
                EndTime = new DateTime(2020, 1, 1, 13, 0, 0),
                PricePerMinute = 0.1m
            }, 6m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "2"},
                StartTime = new DateTime(2020, 1, 1, 12, 0, 0),
                EndTime = new DateTime(2020, 1, 1, 14, 0, 0),
                PricePerMinute = 0.2m
            }, 20m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "3"},
                StartTime = new DateTime(2020, 1, 1, 13, 0, 0),
                EndTime = new DateTime(2020, 1, 1, 13, 10, 0),
                PricePerMinute = 0.3m
            }, 3m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "4"},
                StartTime = new DateTime(2021, 1, 31, 23, 0, 0),
                EndTime = new DateTime(2021, 2, 1, 0, 0, 0),
                PricePerMinute = 0.4m
            }, 20m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "5"},
                StartTime = new DateTime(2019, 1, 1, 12, 0, 0),
                EndTime = new DateTime(2019, 1, 1, 12, 20, 0),
                PricePerMinute = 0.5m
            }, 10m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "6"},
                StartTime = new DateTime(2019, 12, 31, 23, 0, 0),
                EndTime = new DateTime(2020, 1, 1, 1, 0, 0),
                PricePerMinute = 0.5m
            }, 40m},
            new object[] {new RentalPeriod()
            {
                Scooter = new Scooter(){Id = "7"},
                StartTime = new DateTime(2022, 1, 1, 1, 0, 0),
                EndTime = new DateTime(2022, 1, 1, 2, 0, 0),
                PricePerMinute = 0.5m
            }, 20m}
        };
}