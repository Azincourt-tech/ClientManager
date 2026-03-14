# Security & Compliance Notes

## Authentication
Relies on ASP.NET Core Identity or JWT bearer tokens for securing API endpoints.

## Secrets Management
Use `appsettings.json`, environment variables, or .NET User Secrets during local development. Do not check in connection strings containing production passwords.
