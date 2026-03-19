---
status: in_progress
generated: 2026-03-19
agents:
  - type: "test-writer"
    role: "Write unit tests for Customer model"
  - type: "code-reviewer"
    role: "Review test coverage and quality"
docs:
  - "testing-strategy.md"
phases:
  - id: "phase-1"
    name: "Implementation"
    prevc: "E"
    agent: "test-writer"
  - id: "phase-2"
    name: "Validation"
    prevc: "V"
    agent: "test-writer"
---

# Testes da Model Customer Plan

> Adicionar testes unitários para a model Customer no domínio.

## Task Snapshot
- **Primary goal:** Garantir que as regras de negócio da entidade `Customer` estejam cobertas por testes.
- **Success signal:** Arquivo `CustomerTests.cs` criado com testes passando para todos os métodos principais.
- **Key references:**
  - `src/ClientManager.Domain/Model/Customer.cs`
  - `src/ClientManager.Domain/Model/Document.cs`

## Codebase Context
- **Files involved:**
  - `Customer.cs`
  - `CustomerTests.cs` (a ser criado)

## Agent Lineup
| Agent | Role in this plan | Playbook | First responsibility focus |
| --- | --- | --- | --- |
| Test Writer | Implementador | [Test Writer](../agents/test-writer.md) | Criar testes unitários |

## Working Phases

### Phase 1 — Implementation & Iteration
> **Primary Agent:** `test-writer`

**Objective:** Criar a classe de teste e implementar os casos de teste.

**Tasks**

| # | Task | Agent | Status | Deliverable |
|---|------|-------|--------|-------------|
| 1.1 | Criar `CustomerTests.cs` em `tests/ClientManager.Domain.Tests/Model/` | `test-writer` | pending | Arquivo de teste base |
| 1.2 | Testar construtor e limpeza de documento (CPF/CNPJ) | `test-writer` | pending | Testes de inicialização |
| 1.3 | Testar métodos de mudança de status (`Activate`, `Deactivate`, etc.) | `test-writer` | pending | Testes de estado |
| 1.4 | Testar `EvaluateVerificationStatus` para Pessoa Física (PF) | `test-writer` | pending | Testes de regra de negócio PF |
| 1.5 | Testar `EvaluateVerificationStatus` para Pessoa Jurídica (PJ) | `test-writer` | pending | Testes de regra de negócio PJ |

---

### Phase 2 — Validation & Testing
> **Primary Agent:** `test-writer`

**Objective:** Validar a execução dos testes.

**Tasks**

| # | Task | Agent | Status | Deliverable |
|---|------|-------|--------|-------------|
| 2.1 | Executar `dotnet test` focado no projeto de domínio | `test-writer` | pending | Testes passando |

## Success Metrics
- 100% de sucesso nos novos testes unitários.
- Cobertura de todos os caminhos lógicos de `EvaluateVerificationStatus`.
