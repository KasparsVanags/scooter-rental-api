using scooter_rental.Core.Calculators;

namespace scooter_rental.Core.Tests;

public class IncomeCalculatorTests
{
    [Theory]
    [MemberData(nameof(TestDates))]
    public void Income_ValidDates_CalculatesIncome
        (DateTime startTime, DateTime endTime, decimal pricePerMinute, decimal result)
    {
        //Assert
        IncomeCalculator.GetIncome(startTime, endTime, pricePerMinute).Should().Be(result);
    }
    
    public static IEnumerable<object[]> TestDates =>
        new List<object[]>
        {
            //same day
            //below limit
            new object[] { new DateTime(2020, 1, 1), 
                new DateTime(2020, 1, 1, 1, 0, 0), 0.1, 6 },
            //above limit
            new object[] { new DateTime(2020, 1, 1), 
                new DateTime(2020, 1, 1, 1, 0, 0), 1, 20 },
            //2 days
            //below limit
            new object[] { new DateTime(2020, 1, 1, 12, 0, 0), 
                new DateTime(2020, 1, 2, 13, 0, 0), 0.01, 15 },
            //above limit
            new object[] { new DateTime(2020, 1, 1, 12, 0, 0), 
                new DateTime(2020, 1, 2, 13, 0, 0), 1, 40 },
            //partially above limit
            new object[] { new DateTime(2020, 1, 1, 23, 59, 0), 
                new DateTime(2020, 1, 2, 12, 0, 0), 1, 21 },
            //5 days
            //below limit
            new object[] { new DateTime(2020, 1, 1, 12, 0, 0), 
                new DateTime(2020, 1, 5, 13, 0, 0), 0.01, 58.2 },
            //above limit
            new object[] { new DateTime(2020, 1, 1, 12, 0, 0), 
                new DateTime(2020, 1, 5, 13, 0, 0), 1, 100 },
            //partially above limit
            new object[] { new DateTime(2020, 1, 1, 12, 0, 0), 
                new DateTime(2020, 1, 5, 0, 1, 0), 1, 81 },
        };
}