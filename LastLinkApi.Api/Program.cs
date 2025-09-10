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

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "LastLinkApi";
var appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

Console.WriteLine("========================================");
Console.WriteLine($"🚀 STARTING {appName.ToUpper()}");
Console.WriteLine("========================================");
Console.WriteLine($"📦 Application: {appName} v{appVersion}");
Console.WriteLine($"🌍 Environment: {environment}");
Console.WriteLine($"⚡ .NET Version: {Environment.Version}");
Console.WriteLine($"🕒 Started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine($"💻 Machine: {Environment.MachineName}");
Console.WriteLine("========================================");

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
        Description = $"Laslink API",
        Contact = new OpenApiContact
        {
            Name = "LastLink Team",
            Email = "dev@lastlink.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // JWT configuration
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateAdvanceRequestHandler).Assembly));

// Repositories
builder.Services.AddScoped<IAdvanceRequestRepository, AdvanceRequestRepository>();

// JWT Service
builder.Services.AddSingleton<JwtService>(provider =>
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

// ========================================
// CONFIGURAÇÃO DO PIPELINE
// ========================================

Console.WriteLine("📋 Configuring Swagger...");

// Swagger (sempre habilitado)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LastLink API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
    c.DocumentTitle = $"LastLink API Documentation (.NET 9) - {environment}";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
});

Console.WriteLine("✅ Swagger configured successfully!");

// Database
Console.WriteLine("🗄️  Configuring database...");
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    Console.WriteLine("✅ Database configured successfully!");
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check
app.MapGet("/health", () => new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = appVersion,
    dotnetVersion = "9.0",
    environment = environment,
    machineName = Environment.MachineName
});

// JWT token generation
app.MapPost("/auth/token", (JwtService jwtService, string userId) =>
{
    var token = jwtService.GenerateToken(userId, "user");
    return new { token, expiresIn = 3600, userId, environment };
});

// ========================================
// STARTUP FINAL
// ========================================

var urls = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:8080";
Console.WriteLine($"🌐 URLs: {urls}" );

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("========================================");
    Console.WriteLine("🎉 APPLICATION STARTED SUCCESSFULLY!");
    Console.WriteLine("========================================");
    var baseUrl = urls.Replace("0.0.0.0", "localhost").Split(';')[0];
    Console.WriteLine($"🌐 Swagger UI: {baseUrl}/swagger");
    Console.WriteLine($"❤️  Health Check: {baseUrl}/health");
    Console.WriteLine($"🔑 Auth Token: {baseUrl}/auth/token?userId=creator123");
    Console.WriteLine("========================================");
    
    // Abrir Swagger automaticamente em Development
    if (environment == "Development")
    {
        var swaggerUrl = $"{baseUrl}/swagger";
        Console.WriteLine($"🚀 Opening Swagger: {swaggerUrl}");
        
        try
        {
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = swaggerUrl,
                    UseShellExecute = true
                });
                Console.WriteLine("✅ Swagger opened in browser!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Could not open browser: {ex.Message}");
            Console.WriteLine($"📋 Open manually: {swaggerUrl}");
        }
    }
});

app.Run();
