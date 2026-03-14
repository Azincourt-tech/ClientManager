# Architecture Notes

## System Architecture Overview
This is a .NET Core Monolith implementing Domain-Driven Design (DDD) patterns. It connects to a RavenDB document database.

## Architectural Layers
- **Api**: Presentation layer, exposes controllers, REST endpoints and implements global error handling.
- **Application**: Application services, manual DTO mapping (via specialized Extension Classes) and input validation.
- **Domain**: Rich domain models with encapsulated business logic.
- **Domain.Core**: Core interfaces, base classes, and Domain Helpers (CPF, CNPJ, Email).
- **Domain.Services**: Domain-specific services.
- **Infrastructure**: Data access leveraging RavenDB (IAsyncDocumentSession), repository implementations with Soft Delete support, and external integrations.

## Detected Design Patterns
| Pattern | Description |
|---|---|
| Repository | Abstracts data access to RavenDB |
| Rich Domain Model | Business rules and state transitions inside entities |
| Soft Delete | Logical deletion using `IsDeleted` flag in entities and repositories |
| FluentValidation | Declarative validation for DTOs |
| Dependency Injection | Inversion of Control via .NET Core DI |
| Manual Mapping | Extension methods (`ToDto`, `ToModel`) instead of AutoMapper for better performance and control |
| Async/Await | Non-blocking I/O operations throughout the stack |
| ProblemDetails | Standardized API error responses |
| Domain Helpers | `DocumentHelper` (CPF/CNPJ) and `EmailHelper` for core business rules |

## Key Decisions & Trade-offs
Using RavenDB provides schema-less, transactional document storage tuned for .NET. Manual mapping was chosen over AutoMapper to reduce overhead and improve code clarity and maintainability.

