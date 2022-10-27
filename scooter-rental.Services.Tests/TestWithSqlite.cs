using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using scooter_rental.Data;

namespace scooter_rental.Services.Tests;

public abstract class TestWithSqlite : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection;

    protected readonly ScooterRentalDbContext DbContext;

    protected TestWithSqlite()
    {
        _connection = new SqliteConnection(InMemoryConnectionString);
        _connection.CreateFunction("NEWID", () => Guid.NewGuid());
        _connection.Open();
        var options = new DbContextOptionsBuilder<ScooterRentalDbContext>()
            .UseSqlite(_connection)
            .Options;
        DbContext = new ScooterRentalDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Close();
    }
}