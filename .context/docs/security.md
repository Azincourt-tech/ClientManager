# Security & Compliance Notes

## Authentication
Relies on ASP.NET Core Identity or JWT bearer tokens for securing API endpoints.

## Secrets Management
Use `appsettings.json`, environment variables, or .NET User Secrets during local development. Do not check in connection strings containing production passwords.

## File Upload Security
Handled by `IFileValidator`:
- **Content Type Check**: Only `.pdf`, `.png`, `.jpg`, `.jpeg` are allowed.
- **Size Limit**: Maximum 5MB per file.
- **Storage**: Files are stored as attachments in RavenDB, isolated from the filesystem.

## Data Integrity & Validation
- **CPF/CNPJ Validation**: Custom logic implementing official checksum algorithms for Brazilian identity numbers.
- **Rich Domain State**: State transitions (e.g., Verification Status) are controlled within the Domain entities to prevent invalid states.

