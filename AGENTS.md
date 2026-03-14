# AGENTS.md

## Dev environment tips
- Restore dependencies with `dotnet restore` before working on the project.
- Use `dotnet build` to compile the solution.
- Run the API locally using `dotnet run --project src/ShopRavenDb.Api/ShopRavenDb.Api.csproj`.
- Store generated artefacts in `.context/` so reruns stay deterministic.

## Testing instructions
- Execute `dotnet test` to run the test suite for the solution.
- Append `--logger "console;verbosity=detailed"` if you need more verbose test output.
- Trigger `dotnet build && dotnet test` before opening a PR to mimic CI.
- Add or update tests alongside any changes made to Domain logic, Application handlers, or Infrastructure repositories.
- Use `ICpfValidator`, `ICnpjValidator`, and `IFileValidator` for sensitive field and file validations.
- When working with Customers, ensure they support both `Individual` (PF) and `LegalEntity` (PJ) types.

## PR instructions
- Follow Conventional Commits (for example, `feat(domain): add Shop aggregate root` or `fix(api): resolve route conflict`).
- Cross-link new guidelines in `.context/docs/README.md` and playbooks in `.context/agents/README.md` so future agents can find them.
- Respect the architectural boundaries (DDD): Api -> Application -> Domain <- Infrastructure.

## Repository map
- `LICENSE/` — explains the project's licensing terms.
- `README.md/` — the entrypoint for developers summarizing the purpose of the application.
- `ShopRavenDb.sln/` — the root .NET solution file referencing all projects.
- `src/ShopRavenDb.Api/` — the presentation layer containing REST controllers and Global Exception Handling.
- `src/ShopRavenDb.Application/` — the application layer containing application services and FluentValidation logic.
- `src/ShopRavenDb.Domain/`, `src/ShopRavenDb.Domain.Core/`, `src/ShopRavenDb.Domain.Services/` — the core domain layers with entities, interfaces, value objects, and business logic.
- `src/ShopRavenDb.Infrastructure/` — implementation of repositories and connection to RavenDB.

## AI Context References
- Documentation index: `.context/docs/README.md`
- Agent playbooks: `.context/agents/README.md`
- Contributor guide: `CONTRIBUTING.md`
