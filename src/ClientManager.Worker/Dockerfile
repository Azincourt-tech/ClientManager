# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files to restore dependencies
COPY ["src/ClientManager.Worker/ClientManager.Worker.csproj", "src/ClientManager.Worker/"]
COPY ["src/ClientManager.Application/ClientManager.Application.csproj", "src/ClientManager.Application/"]
COPY ["src/ClientManager.Domain/ClientManager.Domain.csproj", "src/ClientManager.Domain/"]
COPY ["src/ClientManager.Domain.Core/ClientManager.Domain.Core.csproj", "src/ClientManager.Domain.Core/"]
COPY ["src/ClientManager.Domain.Services/ClientManager.Domain.Services.csproj", "src/ClientManager.Domain.Services/"]
COPY ["src/ClientManager.Infrastructure/ClientManager.Infrastructure.csproj", "src/ClientManager.Infrastructure/"]
COPY ["src/ClientManager.Infrastructure.Messaging/ClientManager.Infrastructure.Messaging.csproj", "src/ClientManager.Infrastructure.Messaging/"]

RUN dotnet restore "src/ClientManager.Worker/ClientManager.Worker.csproj"

# Copy the rest of the code and build
COPY . .
WORKDIR "/src/src/ClientManager.Worker"
RUN dotnet publish "ClientManager.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClientManager.Worker.dll"]
