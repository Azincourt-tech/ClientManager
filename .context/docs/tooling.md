# Tooling & Productivity Guide

## IDE Setup
- ReSharper or JetBrains Rider is recommended for working with the DDD structure.
- Prettier/EditorConfig to maintain C# formatting standards.

## Database tooling
- **RavenDB Management Studio**: Accessible at `http://localhost:8080` for local dev.
- **Docker & Docker Compose**: Used to orchestrate the local database environment (RavenDB and RabbitMQ).

## Messaging & Email
- **CloudAMQP**: Managed RabbitMQ for production messaging.
- **RabbitMQ (Local)**: Runs via Docker for development.
- **SendGrid**: Transactional email service for production.
- **Mailtrap**: SMTP sandbox for testing emails in development without sending real messages.

