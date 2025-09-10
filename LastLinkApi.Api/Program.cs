using System.Reflection;
using Microsoft.OpenApi.Models;
using LastLinkApi.Application.Commands;
using LastLinkApi.Domain.Repositories; // Corrigido: Handlers não são necessários aqui
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

    // Incluir XML de documentação, se existir
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // JWT Bearer config (mock por enquanto)
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

// MediatR
builder.Services.AddMediatR(typeof(CreateAdvanceRequestCommand).Assembly);

// Repositório (InMemory por enquanto)
builder.Services.AddSingleton<IAdvanceRequestRepository, InMemoryAdvanceRequestRepository>();

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

// Swagger UI
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

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

// Health check endpoint (sem .WithOpenApi no .NET 8)
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
