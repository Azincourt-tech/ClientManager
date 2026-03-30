---
slug: project-structure
category: architecture
generatedAt: 2026-03-17T10:00:00.000Z
---

# Como o código está organizado?

## Estrutura do Projeto

O projeto segue os princípios da **Arquitetura Cebola (Onion Architecture)** e **DDD (Domain-Driven Design)**:

- **src/**
    - `ClientManager.Api`: Camada de apresentação (Controllers, Middlewares).
    - `ClientManager.Application`: Serviços de aplicação, DTOs, Mappers e Validators.
    - `ClientManager.Domain`: Entidades de negócio, Enums e lógica de domínio.
    - `ClientManager.Domain.Core`: Interfaces base, helpers de domínio (CPF, CNPJ) e respostas compartilhadas.
    - `ClientManager.Domain.Services`: Implementações de serviços de domínio.
    - `ClientManager.Infrastructure`: Persistência de dados (RavenDB), repositórios e injeção de dependência.

- **tests/**
    - Projetos de testes unitários para cada camada lógica da aplicação utilizando **xUnit** e **Moq**.
