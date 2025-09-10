using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using LastLinkApi.Infrastructure.Data;
using LastLinkApi.Infrastructure.Repositories;
using LastLinkApi.Infrastructure.Services;
using LastLinkApi.Domain.Repositories;
using LastLinkApi.Application.Handlers;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LastLink API",
        Version = "v1",
        Description = "REST API para gestão de solicitações de antecipação de recebíveis",
        Contact = new OpenApiContact
        {
            Name = "Equipe LastLink",
            Email = "dev@lastlink.com"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticação via JWT usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(typeof(CreateAdvanceRequestHandler).Assembly);

// Repositories
builder.Services.AddScoped<IAdvanceRequestRepository, AdvanceRequestRepository>();

// Services
builder.Services.AddScoped<JwtService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new JwtService(
        configuration["Jwt:SecretKey"] ?? "your-super-secure-secret-key-with-at-least-32-characters",
        configuration["Jwt:Issuer"] ?? "LastLinkApi",
        configuration["Jwt:Audience"] ?? "LastLinkApi",
        int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60")
    );
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "your-super-secure-secret-key-with-at-least-32-characters";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "LastLinkApi",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"] ?? "LastLinkApi",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LastLink API v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "LastLink API Swagger";
    });
}

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health endpoint (sem .WithOpenApi no .NET 8)
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
})
.WithName("HealthCheck")
.WithTags("Health");

// Auth mock endpoint (sem .WithOpenApi no .NET 8)
app.MapPost("/auth/token", (string userId) =>
{
    var mockToken = Convert.ToBase64String(
        System.Text.Encoding.UTF8.GetBytes($"mock-token-{userId}-{DateTime.UtcNow:yyyyMMddHHmmss}")
    );
    return new { token = mockToken, expiresIn = 3600, userId };
})
.WithName("GenerateToken")
.WithTags("Authentication");

app.Run();
