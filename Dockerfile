# Multi-stage build for image optimization
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["LastLinkApi.Api/LastLinkApi.Api.csproj", "LastLinkApi.Api/"]
COPY ["LastLinkApi.Application/LastLinkApi.Application.csproj", "LastLinkApi.Application/"]
COPY ["LastLinkApi.Domain/LastLinkApi.Domain.csproj", "LastLinkApi.Domain/"]
COPY ["LastLinkApi.Infrastructure/LastLinkApi.Infrastructure.csproj", "LastLinkApi.Infrastructure/"]

RUN dotnet restore "LastLinkApi.Api/LastLinkApi.Api.csproj"

# Copy all source code
COPY . .

# Build application
WORKDIR "/src/LastLinkApi.Api"
RUN dotnet build "LastLinkApi.Api.csproj" -c Release -o /app/build

# Publish application
RUN dotnet publish "LastLinkApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image - runtime only
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directory for SQLite database
RUN mkdir -p /app/data

# Copy published files
COPY --from=build /app/publish .

# Configure environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/lastlink.db"

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Startup command
ENTRYPOINT ["dotnet", "LastLinkApi.Api.dll"]