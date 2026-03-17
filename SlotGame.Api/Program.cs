using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SlotGame.Api.Constants;
using SlotGame.Api.Data;
using SlotGame.Api.Infrastructure;
using SlotGame.Api.Services;
using SlotGame.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(ErrorMessages.ConnectionStringNotFound);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ISpinEngineService, SpinEngineService>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateGameRequestValidator>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

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
