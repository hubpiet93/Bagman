using FluentValidation;
using TypowanieMeczy.Domain.Interfaces;
using TypowanieMeczy.Domain.Services;
using TypowanieMeczy.Infrastructure.Services;
using TypowanieMeczy.Infrastructure.Repositories;
using TypowanieMeczy.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
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

// Add Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Supabase:Url"];
        options.Audience = builder.Configuration["Supabase:AnonKey"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

// Add Repositories
builder.Services.AddScoped<ITableRepository, SupabaseTableRepository>();
builder.Services.AddScoped<IMatchRepository, SupabaseMatchRepository>();
builder.Services.AddScoped<IBetRepository, SupabaseBetRepository>();
builder.Services.AddScoped<IPoolRepository, SupabasePoolRepository>();
builder.Services.AddScoped<IUserRepository, SupabaseUserRepository>();
builder.Services.AddScoped<IStatisticsRepository, SupabaseStatisticsRepository>();

// Add Domain Services
builder.Services.AddScoped<IAuthService, SupabaseAuthService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IBetService, BetService>();
builder.Services.AddScoped<IPoolService, PoolService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// Add Infrastructure
builder.Services.AddScoped<ISupabaseClient, SupabaseClient>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Background Services
builder.Services.AddHostedService<MatchManagementBackgroundService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");

app.UseAuthentication();
app.UseAuthorization();

// Add custom middleware
app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run(); 