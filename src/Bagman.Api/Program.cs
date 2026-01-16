using Bagman.Api.Middleware;
using Bagman.Api.Validators;
using Bagman.Infrastructure;
using Bagman.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:5002")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddApplicationDbContext(builder.Configuration.GetConnectionString("Postgres"));
builder.Services.AddDomainServices();
// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bagman API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");

// Add validation exception middleware BEFORE authorization
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class public for integration tests
public partial class Program
{
}
