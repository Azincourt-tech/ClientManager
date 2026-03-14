# Architecture Notes

## System Architecture Overview
This is a .NET Core Monolith implementing Domain-Driven Design (DDD) patterns. It connects to a RavenDB document database.

## Architectural Layers
- **Api**: Presentation layer, exposes controllers, REST endpoints and implements global error handling.
- **Application**: Application services, DTO mapping and input validation.
- **Domain**: Rich domain models with encapsulated business logic.
- **Domain.Core**: Core interfaces and base classes.
- **Domain.Services**: Domain-specific services.
- **Infrastructure**: Data access leveraging RavenDB (IAsyncDocumentSession), repository implementations, and external integrations.

## Detected Design Patterns
| Pattern | Description |
|---|---|
| Repository | Abstracts data access to RavenDB |
| Rich Domain Model | Business rules and state transitions inside entities |
| FluentValidation | Declarative validation for DTOs |
| Dependency Injection | Inversion of Control via .NET Core DI |
| Async/Await | Non-blocking I/O operations throughout the stack |
| ProblemDetails | Standardized API error responses |

## Key Decisions & Trade-offs
Using RavenDB provides schema-less, transactional document storage tuned for .NET.
