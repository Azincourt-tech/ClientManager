# Architecture Notes

## System Architecture Overview
This is a .NET Core Monolith implementing Domain-Driven Design (DDD) patterns. It connects to a RavenDB document database.

## Architectural Layers
- **Api**: Presentation layer, exposes controllers and REST endpoints.
- **Application**: Application services and CQRS handlers.
- **Domain**: Domain models, aggregates, entities, and value objects.
- **Domain.Core**: Core interfaces and base classes.
- **Domain.Services**: Domain-specific services containing business logic.
- **Infrastructure**: Data access leveraging RavenDB, repository implementations, and external integrations.

## Detected Design Patterns
| Pattern | Description |
|---|---|
| Repository | Abstracts data access to RavenDB |
| CQRS | Separates read and write operations in Application tier |
| Dependency Injection | Inversion of Control via .NET Core DI |

## Key Decisions & Trade-offs
Using RavenDB provides schema-less, transactional document storage tuned for .NET.
