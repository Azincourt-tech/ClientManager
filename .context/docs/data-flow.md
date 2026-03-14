# Data Flow & Integrations

## Data Flow & Integrations
Data flows from API Controllers -> Application Layer (CQRS/Services) -> Domain Objects -> Infrastructure (Repositories) -> RavenDB.

## Module Dependencies
- **Api** -> Application, Infrastructure
- **Application** -> Domain, Domain.Core, Domain.Services
- **Domain.Services** -> Domain
- **Infrastructure** -> Domain, Domain.Core

## External Integrations
- RavenDB database cluster.
