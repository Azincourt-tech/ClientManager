# Development Workflow

## Local Development
- IDE: Visual Studio 2022 or JetBrains Rider
- **Database**: Run `docker-compose up -d` to start a local RavenDB instance.
- **Setup**: Create a database named `ClientManagementDB` in the RavenDB Studio (`http://localhost:8080`).
- Run `dotnet restore && dotnet build` to compile the solution.
- Run the API project (`ClientManager.Api`) using IIS Express or Kestrel.

## Environments Strategy
- **Development**: Uses `appsettings.Development.json` pointing to `http://localhost:8080` (Docker). No certificates required.
- **Production**: Uses `appsettings.json` with placeholders for RavenDB Cloud. Requires a `.pfx` certificate and secure URL.

## Branching & Releases
Use feature branches and create Pull Requests targeting the `main` branch. 
Ensure code passes all CI tests.

### Commit Standards
Follow **Conventional Commits**:
- `feat(domain): add customer aggregate`
- `fix(api): resolve route conflict`
- `docs(context): update glossary`

