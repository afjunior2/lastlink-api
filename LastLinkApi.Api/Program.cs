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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer; // 👈 necessário para versioning no swagger

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "LastLinkApi";
var appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

// ========================================
// LOG DE INICIALIZAÇÃO
// ========================================
Console.WriteLine("========================================");
Console.WriteLine($"🚀 STARTING {appName.ToUpper()}");
Console.WriteLine("========================================");
Console.WriteLine($"📦 Application: {appName} v{appVersion}");
Console.WriteLine($"🌍 Environment: {environment}");
Console.WriteLine($"⚡ .NET Version: {Environment.Version}");
Console.WriteLine($"🕒 Started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine($"💻 Machine: {Environment.MachineName}");
Console.WriteLine("========================================");

// ========================================
// SERVICES
// ========================================
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1, v2...
    options.SubstituteApiVersionInUrl = true;
});

// Explorer para Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o Swagger para lidar com múltiplas versões
builder.Services.ConfigureOptions<SwaggerConfig>();

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
// PIPELINE
// ========================================
Console.WriteLine("📋 Configuring Swagger...");

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            $"LastLink API {description.GroupName.ToUpper()}");
    }
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = $"LastLink API Documentation (.NET 9) - {environment}";
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
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
    environment,
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
});

app.Run();

// ========================================
// CONFIGURAÇÃO EXTRA DO SWAGGER
// ========================================
public class SwaggerConfig : Microsoft.Extensions.Options.IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerConfig(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "LastLink API",
                Version = description.ApiVersion.ToString(),
                Description = "API para gestão de solicitações de antecipação de recebíveis",
                Contact = new OpenApiContact
                {
                    Name = "LastLink Team",
                    Email = "dev@lastlink.com"
                }
            });
        }
    }
}
