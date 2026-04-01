# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Directory.Build.props first (provides TargetFramework for all projects)
COPY ["Directory.Build.props", "./"]

# Copy all project files to restore dependencies
COPY ["src/ClientManager.Api/ClientManager.Api.csproj", "src/ClientManager.Api/"]
COPY ["src/ClientManager.Application/ClientManager.Application.csproj", "src/ClientManager.Application/"]
COPY ["src/ClientManager.Domain/ClientManager.Domain.csproj", "src/ClientManager.Domain/"]
COPY ["src/ClientManager.Domain.Core/ClientManager.Domain.Core.csproj", "src/ClientManager.Domain.Core/"]
COPY ["src/ClientManager.Domain.Services/ClientManager.Domain.Services.csproj", "src/ClientManager.Domain.Services/"]
COPY ["src/ClientManager.Infrastructure/ClientManager.Infrastructure.csproj", "src/ClientManager.Infrastructure/"]
COPY ["src/ClientManager.Infrastructure.Messaging/ClientManager.Infrastructure.Messaging.csproj", "src/ClientManager.Infrastructure.Messaging/"]
COPY ["ClientManager.sln", "./"]

# Restore dependencies using full path from WORKDIR /src
RUN dotnet restore "src/ClientManager.Api/ClientManager.Api.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/src/ClientManager.Api"
RUN dotnet publish "ClientManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render defaults
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ClientManager.Api.dll"]

# Force rebuild: 2026-04-01
