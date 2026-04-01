# ClientManager - Customer & Document Management API

A REST API for managing customers and their documents, built with .NET 9 following Clean Architecture and DDD principles. Designed as a study project for unit testing and architectural patterns, using RavenDB as the NoSQL database.

---

## Features

- **Customer management** (CRUD) with addresses, CPF/CNPJ validation
- **Document management** with type categorization and expiration dates
- **User authentication** with JWT tokens and refresh tokens
- **Role-based authorization** (Admin, Manager, Viewer)
- **File upload** with extension and size validation (IFileValidator)
- **Soft Delete** on all main entities
- **Client verification status** (Verified, Attention, Pending) based on documents
- **Multi-profile support** for PF (individual) and PJ (company)
- **Email notifications** via Resend (production) or Mailtrap (development)
- **Code quality infrastructure** with centralized build configuration and analyzers
- **Comprehensive unit tests** (114 tests, 0 failures)

---

## Architecture

The project follows **Onion Architecture** and **DDD (Domain-Driven Design)**:

```
ClientManager/
├── src/
│   ├── ClientManager.Api/              # Entry layer (Controllers, Middlewares, DI)
│   ├── ClientManager.Application/      # Business orchestration (DTOs, FluentValidation, Mappers)
│   ├── ClientManager.Domain/           # Rich Domain Model (Entities, Interfaces, Rules)
│   └── ClientManager.Infrastructure/   # Technical details (RavenDB, JWT, Email, PDF)
└── tests/
    ├── ClientManager.Domain.Tests/
    ├── ClientManager.Application.Tests/
    └── ClientManager.Infrastructure.Tests/
```

### Key Design Decisions

- **Rich Domain Model** with encapsulated state and domain validation
- **Manual DTO mapping** via Extension Methods (no AutoMapper)
- **FluentValidation** for input validation
- **Async operations** throughout with `IAsyncDocumentSession`
- **BCrypt** for secure password hashing
- **AES-256-GCM** encryption for sensitive tokens
- **EditorConfig** + **Directory.Build.props** for centralized code style

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | [.NET 9.0](https://dotnet.microsoft.com/) |
| Database | [RavenDB](https://ravendb.net/) (NoSQL with attachments) |
| Authentication | JWT (JSON Web Token) |
| Authorization | Role-based (Admin, Manager, Viewer) |
| Password Hashing | BCrypt |
| Email | [Resend](https://resend.com/) (production) / Mailtrap (development) |
| Testing | xUnit, Moq, FluentAssertions |
| Code Quality | .editorconfig, Directory.Build.props, NetAnalyzers |
| CI/CD | GitHub Actions |
| Deploy | [Render](https://render.com/) + Docker |

---

## Setup

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/) (optional, for local services)
- RavenDB instance (local Docker or cloud)

### 1. Start Local Services

### 2. Configure Environment

Set up user secrets for development:

```bash
# In src/ClientManager.Api directory
dotnet user-secrets set "Smtp:Username" "your_mailtrap_username"
dotnet user-secrets set "Smtp:Password" "your_mailtrap_password"

# JWT Configuration (generate a new secret: openssl rand -hex 32)
dotnet user-secrets set "Jwt:Key" "$(openssl rand -hex 32)"
dotnet user-secrets set "Jwt:Issuer" "ClientManager"
dotnet user-secrets set "Jwt:Audience" "ClientManager"
dotnet user-secrets set "Jwt:ExpireMinutes" "60"
```

### 3. Database Setup

Create a database named **`ClientManagerDB`** in your RavenDB instance.

### 4. Run

```bash
# Run the API
dotnet run --project src/ClientManager.Api/ClientManager.Api.csproj
```

### 5. API Documentation

With the API running:

- **Swagger UI:** `https://localhost:7023/swagger`
- **Scalar Docs:** `https://localhost:7023/api-docs`

---

## Authentication & Authorization

The system uses **JWT** for authentication with three access levels:

| Role | Description |
|------|-------------|
| Admin | Full access, including user management |
| Manager | Create and edit customers and documents |
| Viewer | Read-only access to customers and documents |

### Auth Endpoints (`/api/auth`)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/auth/register` | Register new user (requires Admin) |
| POST | `/api/auth/login` | Login and get JWT token |
| POST | `/api/auth/refresh` | Refresh expired token |

### User Endpoints (`/api/user`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/api/user` | Admin | List all users |
| GET | `/api/user/{id}` | Admin | Get user by ID |
| PUT | `/api/user/{id}` | Admin | Update user |
| DELETE | `/api/user/{id}` | Admin | Delete user |

---

## Tests

```bash
# Run all tests
dotnet test
```

**114 unit tests** covering:

- **Controllers:** Status codes, response flow, and authorization
- **Application:** DTO mapping and authentication logic
- **Domain Services:** Business rules (email validation, client activation, user management)
- **Models:** Entity integrity including the User entity

---

## Code Quality

The project includes centralized code quality infrastructure:

- **`.editorconfig`** - Standardizes formatting across all editors
- **`Directory.Build.props`** - Centralizes .NET version and shared build settings
- **NetAnalyzers** - Static analysis with `EnforceCodeStyleInBuild` enabled
- **`TreatWarningsAsErrors`** in Release mode

---

## Production Configuration

For production deployment (Render):

```bash
# Required environment variables
ConnectionStrings__RavenDB      # RavenDB cloud URL
Resend__ApiKey                  # Resend API key
Resend__FromEmail               # Verified sender email
Jwt__Key                        # JWT signing key
```

---

## License

MIT
