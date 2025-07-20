using Bagman.Api.Middleware;
using Bagman.Api.Validators;
using Bagman.Domain.Repositories;
using Bagman.Domain.Services;
using Bagman.Infrastructure.Repositories;
using Bagman.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Supabase;

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

// Configure Supabase
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];

// Log configuration for debugging
Console.WriteLine($"Supabase URL: {supabaseUrl}");
Console.WriteLine($"Supabase Key: {supabaseKey?.Substring(0, Math.Min(20, supabaseKey?.Length ?? 0))}...");

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
    throw new InvalidOperationException("Supabase configuration is missing. Please check appsettings.json");

// Validate URL format
if (!Uri.TryCreate(supabaseUrl, UriKind.Absolute, out var uri) || uri.Scheme != "https")
    throw new InvalidOperationException($"Invalid Supabase URL: {supabaseUrl}. URL must be a valid HTTPS URL.");

builder.Services.AddSingleton<Client>(provider => new Client(supabaseUrl, supabaseKey));

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add Authorization (without JWT Bearer - we'll use custom middleware with Supabase)
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
public partial class Program { }
