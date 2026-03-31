# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todos os arquivos .csproj primeiro para restaurar as dependências
COPY ["src/ClientManager.Api/ClientManager.Api.csproj", "src/ClientManager.Api/"]
COPY ["src/ClientManager.Application/ClientManager.Application.csproj", "src/ClientManager.Application/"]
COPY ["src/ClientManager.Domain/ClientManager.Domain.csproj", "src/ClientManager.Domain/"]
COPY ["src/ClientManager.Domain.Core/ClientManager.Domain.Core.csproj", "src/ClientManager.Domain.Core/"]
COPY ["src/ClientManager.Domain.Services/ClientManager.Domain.Services.csproj", "src/ClientManager.Domain.Services/"]
COPY ["src/ClientManager.Infrastructure/ClientManager.Infrastructure.csproj", "src/ClientManager.Infrastructure/"]
COPY ["src/ClientManager.Infrastructure.Messaging/ClientManager.Infrastructure.Messaging.csproj", "src/ClientManager.Infrastructure.Messaging/"]

# Restaurar dependências
RUN dotnet restore "src/ClientManager.Api/ClientManager.Api.csproj"

# Copiar o restante do código e compilar
COPY . .
WORKDIR "/src/src/ClientManager.Api"
RUN dotnet build "ClientManager.Api.csproj" -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish "ClientManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render define a variável PORT automaticamente.
ENV ASPNETCORE_URLS=http://+:${PORT:-10000}
EXPOSE 10000

ENTRYPOINT ["dotnet", "ClientManager.Api.dll"]
