---
slug: getting-started
category: getting-started
generatedAt: 2026-03-17T10:00:00.000Z
---

# Como eu configuro e rodo este projeto?

## Pré-requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Instalação e Execução

### 1. Clonar o repositório
```bash
git clone <repository-url>
cd ClientManager
```

### 2. Subir o Banco de Dados (RavenDB)
```bash
docker-compose up -d
```
Acesse o RavenDB Studio em `http://localhost:8080` e crie um banco chamado `ClientManagementDB`.

### 3. Rodar a API
```bash
dotnet run --project src/ClientManager.Api/ClientManager.Api.csproj
```

### 4. Acessar a Documentação
- **Swagger:** `https://localhost:7023/swagger`
- **Scalar:** `https://localhost:7023/api-docs`
