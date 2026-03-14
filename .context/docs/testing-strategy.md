# Testing Strategy

## Frameworks
Uses `xUnit` or `NUnit`, along with `Moq` or NSubstitute for mocking.

## Test Types
- **Unit Tests**: Test domain services and application handlers in isolation.
- **Integration Tests**: Test the API and Infrastructure components using a test or embedded instance of RavenDB.

Run tests via Command Line (`dotnet test`) or Test Explorer in Visual Studio.

