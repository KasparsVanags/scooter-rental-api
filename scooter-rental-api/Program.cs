using Microsoft.EntityFrameworkCore;
using scooter_rental.Core.Interfaces;
using scooter_rental.Core.Models;
using scooter_rental.Core.Models.ScooterValidators;
using scooter_rental.Core.Services;
using scooter_rental.Data;
using scooter_rental.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ScooterRentalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("scooter-rental"));
});

builder.Services.AddScoped<IScooterRentalDbContext, ScooterRentalDbContext>();

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddScoped<IEntityService<Scooter>, EntityService<Scooter>>();
builder.Services.AddScoped<IRentalService, RentalService>();

builder.Services.AddScoped<IScooterValidator, ScooterPricePerMinuteValidator>();
builder.Services.AddScoped<IScooterValidator, ScooterIdValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();