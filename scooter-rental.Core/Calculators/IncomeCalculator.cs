using Microsoft.Extensions.Configuration;

namespace scooter_rental.Core.Calculators;

public static class IncomeCalculator
{
    public static decimal GetIncome(DateTime startTime, DateTime endTime, decimal pricePerMinute)
    {
        var maxRentCostPerDay = int.Parse(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build().GetSection("Constants")["MaxRentCostPerDay"]);
        decimal income = 0;
        var daysBetween = Math.Ceiling((endTime.Date - startTime.Date).TotalDays - 1);
        if (startTime.Date.Equals(endTime.Date))
        {
            income = (decimal)(endTime - startTime).TotalMinutes * pricePerMinute;
            return income > maxRentCostPerDay ? maxRentCostPerDay : income;
        }

        var firstDayIncome = (decimal)(startTime.Date.AddDays(1) - startTime).TotalMinutes * pricePerMinute;
        var lastDayIncome = (decimal)(endTime - endTime.Date).TotalMinutes * pricePerMinute;
        income += firstDayIncome > maxRentCostPerDay ? maxRentCostPerDay : firstDayIncome;
        income += lastDayIncome > maxRentCostPerDay ? maxRentCostPerDay : lastDayIncome;
        for (var i = 0; i < daysBetween; i++)
        {
            const int minutesInADay = 1440;
            var incomePerDay = minutesInADay * pricePerMinute;
            income += incomePerDay > maxRentCostPerDay ? maxRentCostPerDay : incomePerDay;
        }

        return income;
    }
}