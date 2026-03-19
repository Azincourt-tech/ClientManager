---
status: in_progress
progress: 100
generated: 2026-03-19
agents:
  - type: "backend-specialist"
    role: "Implement endpoints and application logic"
  - type: "test-writer"
    role: "Write unit tests for new endpoints"
  - type: "code-reviewer"
    role: "Review changes for architectural consistency"
docs:
  - "project-overview.md"
  - "architecture.md"
phases:
  - id: "phase-1"
    name: "Application & API Implementation"
    prevc: "E"
    agent: "backend-specialist"
  - id: "phase-2"
    name: "Validation & Testing"
    prevc: "V"
    agent: "test-writer"
lastUpdated: "2026-03-19T02:19:58.773Z"
---

# Melhoria de Endpoints de Documento Plan

> Implementação de melhorias nos endpoints de documentos, incluindo listagem e contagem por cliente.

## Task Snapshot
- **Primary goal:** Expor funcionalidades de listagem e contagem de documentos por cliente na API.
- **Success signal:** Endpoints `GET api/document/customer/{customerId}` e `GET api/document/customer/{customerId}/count` funcionando e testados.
- **Key references:**
  - [Documentation Index](../docs/README.md)
  - `src/ClientManager.Api/Controllers/DocumentController.cs`
  - `src/ClientManager.Application/DocumentApplication.cs`

## Codebase Context
- **Files involved:**
  - `IDocumentApplication.cs`
  - `DocumentApplication.cs`
  - `DocumentController.cs`
  - `DocumentControllerTests.cs`

## Agent Lineup
| Agent | Role in this plan | Playbook | First responsibility focus |
| --- | --- | --- | --- |
| Backend Specialist | Implementador | [Backend Specialist](../agents/backend-specialist.md) | Alterar Application e Controller |
| Test Writer | Validador | [Test Writer](../agents/test-writer.md) | Criar testes unitários para os novos endpoints |

## Working Phases

### Phase 1 — Implementation & Iteration
> **Primary Agent:** `backend-specialist`

**Objective:** Implementar os métodos na camada de Application e os endpoints no Controller.

**Tasks**

| # | Task | Agent | Status | Deliverable |
|---|------|-------|--------|-------------|
| 1.1 | Adicionar `GetDocumentsByCustomerIdAsync` em `IDocumentApplication` | `backend-specialist` | pending | Interface atualizada |
| 1.2 | Implementar `GetDocumentsByCustomerIdAsync` em `DocumentApplication` | `backend-specialist` | pending | Implementação com mapeamento para DTO |
| 1.3 | Adicionar endpoints no `DocumentController` | `backend-specialist` | pending | Novos métodos decorados com atributos de rota |

---

### Phase 2 — Validation & Testing
> **Primary Agent:** `test-writer`

**Objective:** Garantir que os novos endpoints funcionam conforme esperado.

**Tasks**

| # | Task | Agent | Status | Deliverable |
|---|------|-------|--------|-------------|
| 2.1 | Adicionar testes para listagem em `DocumentControllerTests.cs` | `test-writer` | pending | Testes passando |
| 2.2 | Adicionar testes para contagem em `DocumentControllerTests.cs` | `test-writer` | pending | Testes passando |
| 2.3 | Executar suite completa de testes | `test-writer` | pending | Relatório de `dotnet test` |

## Success Metrics
- Cobertura de testes para os novos endpoints.
- Respostas da API seguindo o padrão `ServiceResponse` e `ApiOkResult`.

## Execution History

> Last updated: 2026-03-19T02:19:58.773Z | Progress: 100%

### phase-1 [DONE]
- Started: 2026-03-19T02:19:42.822Z
- Completed: 2026-03-19T02:19:58.773Z

- [x] Step 1: Step 1 *(2026-03-19T02:19:42.822Z)*
  - Notes: Implementação concluída e validada com testes unitários.
- [x] Step 2: Step 2 *(2026-03-19T02:19:53.747Z)*
- [x] Step 3: Step 3 *(2026-03-19T02:19:58.773Z)*
