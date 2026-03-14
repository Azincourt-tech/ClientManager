# ClientManager - Gestão de Clientes e Documentos

Este projeto foi desenvolvido como um modelo de estudo para **Testes Unitários** e **Arquitetura Limpa (Clean Architecture)**, utilizando o banco de dados NoSQL **RavenDB**.

A API permite o cadastro completo de clientes (incluindo endereços) e o gerenciamento de documentos anexos (como PDFs ou imagens) armazenados diretamente no banco de dados.

O projeto segue os princípios da **Arquitetura Cebola (Onion Architecture)** e **DDD (Domain-Driven Design)**, garantindo desacoplamento e alta testabilidade:

*   **ClientManager.Api**: Camada de entrada, contém os Controllers, Middlewares de Exception Global e configurações de injeção de dependência.
*   **ClientManager.Application**: Orquestração da lógica, mapeamento de DTOs e Validações de entrada (**FluentValidation**).
*   **ClientManager.Domain**: Entidades de negócio ricas (**Rich Domain Model**) com encapsulamento de estado.
*   **ClientManager.Domain.Services**: Implementação das regras de negócio.
*   **ClientManager.Infrastructure**: Detalhes técnicos, persistência **assíncrona** no RavenDB e extensões.
*   **Tests**: Projetos de testes unitários para cada camada lógica da aplicação.

## 🛠️ Tecnologias e Padrões Aplicados

*   **Framework:** .NET 9.0
*   **Banco de Dados:** RavenDB (NoSQL com suporte a anexos).
*   **Operações Assíncronas:** Uso extensivo de `async/await` e `IAsyncDocumentSession` para alta performance.
*   **Validação de Identidade:** Validadores reais para **CPF** e **CNPJ**.
*   **Gestão de Documentos:** Categorização por tipo e controle de **Data de Expiração**.
*   **Segurança de Upload:** Políticas de extensão e tamanho via `IFileValidator`.
*   **Status de Verificação:** Lógica automática para classificar clientes como `Verified`, `Attention` ou `Active` com base nos documentos.
*   **Suporte Multi-perfil:** Tratamento diferenciado para **Pessoa Física (PF)** e **Pessoa Jurídica (PJ)**.
*   **Documentação:** Swagger e Scalar (Modern API Docs).
*   **Testes:** xUnit, Moq (Mocking) e FluentAssertions.

## 🚀 Como Rodar o Projeto

### 1. Pré-requisitos
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
*   Instância do **RavenDB** rodando localmente (Porta 8080).

**Dica: Rode o RavenDB rapidamente via Docker:**
```bash
docker run -d -p 8080:8080 -p 38888:38888 ravendb/ravendb
```

### 2. Configuração do Banco de Dados
1.  Acesse o painel do RavenDB em `http://localhost:8080`.
2.  Crie um novo banco de dados chamado: **`Shop`**.

### 3. Execução da API
No terminal, na raiz do projeto, execute:
```bash
dotnet run --project src/ClientManager.Api/ClientManager.Api.csproj
```

### 4. Acessando a Documentação
Com a API rodando, você pode testar os endpoints através de:
*   **Swagger UI:** `https://localhost:7023/swagger`
*   **Scalar Docs:** `https://localhost:7023/api-docs` (Interface moderna)

## 🧪 Como Rodar os Testes

Este projeto foi focado em testabilidade. Para rodar todos os testes unitários e verificar a integridade do sistema, use o comando:

```bash
dotnet test
```

Os testes cobrem:
*   **Controllers:** Status codes e fluxo de retorno.
*   **Application:** Mapeamento correto de DTOs.
*   **Domain Services:** Regras de negócio como validação de e-mail e ativação de cliente.
*   **Models:** Integridade das entidades.

---
Desenvolvido para fins de estudo de arquitetura e qualidade de software.

