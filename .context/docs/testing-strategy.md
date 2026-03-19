# Testing Strategy

## Overview
The ClientManager project prioritizes a **Pyramid Testing Strategy**, focusing heavily on **Unit Tests** for business logic isolation, while providing Integration and API tests for critical flow verification.

## Testing Stack
- **xUnit**: Principal framework for test execution in the .NET ecosystem.
- **Moq**: Used for mocking and stubbing dependencies (Repositories, RavenDB Sessions).
- **FluentAssertions**: Provides expressive and human-readable assertions.
- **Microsoft.AspNetCore.Mvc.Testing**: Used for integration testing of API endpoints.

## Test Projects & Scope

| Project | Target Layer | Goal |
|---|---|---|
| **ClientManager.Api.Tests** | `Api` | Validate HTTP status codes, routing, and controller responses using `WebApplicationFactory`. |
| **ClientManager.Application.Tests** | `Application` | Test orchestration logic, manual DTO mapping (Mappers), and Input Validation. |
| **ClientManager.Domain.Services.Tests** | `Domain.Services` | Verify cross-cutting domain rules and multi-entity interactions. |
| **ClientManager.Domain.Tests** | `Domain` | Test rich domain models, state transitions, and core business rules (Encapsulation). |

## Key Testing Patterns
- **AAA (Arrange, Act, Assert)**: Standard structure for all test cases.
- **Naming Convention**: `[MethodName]_[Scenario]_[ExpectedResult]`.
- **Isolation**: Heavy use of `Moq` to ensure that unit tests do not depend on external infrastructure (Databases, File System).
- **Automation**: Tests are integrated into the CI/CD pipeline (GitHub Actions).

## How to Execute
Execute all tests from the root directory:
```bash
dotnet test
```

For detailed coverage reports (if installed):
```bash
dotnet test --collect:"XPlat Code Coverage"
```

