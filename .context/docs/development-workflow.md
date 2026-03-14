# Development Workflow

## Local Development
- IDE: Visual Studio 2022 or JetBrains Rider
- Run `dotnet restore && dotnet build` to compile the solution.
- Run the API project (`ShopRavenDb.Api`) using IIS Express or Kestrel.

## Branching & Releases
Use feature branches and create Pull Requests targeting the `main` branch. 
Ensure code passes all CI tests.

### Commit Standards
Follow **Conventional Commits**:
- `feat(domain): add customer aggregate`
- `fix(api): resolve route conflict`
- `docs(context): update glossary`
