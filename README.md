# ClientManager - Gestão de Clientes e Documentos

Este projeto foi desenvolvido como um modelo de estudo para **Testes Unitários** e **Arquitetura Limpa (Clean Architecture)**, utilizando o banco de dados NoSQL **RavenDB**.

A API permite o cadastro completo de clientes (incluindo endereços) e o gerenciamento de documentos anexos (como PDFs ou imagens) armazenados diretamente no banco de dados.

O projeto segue os princípios da **Arquitetura Cebola (Onion Architecture)** e **DDD (Domain-Driven Design)**, garantindo desacoplamento e alta testabilidade:

*   **ClientManager.Api**: Camada de entrada, contém os Controllers, Middlewares de Exception Global e configurações de injeção de dependência.
*   **ClientManager.Application**: Orquestração da lógica, mapeamento manual de DTOs (Extension Methods) e Validações de entrada (**FluentValidation**).
*   **ClientManager.Domain**: Entidades de negócio ricas (**Rich Domain Model**) com encapsulamento de estado e suporte a **Soft Delete**.
*   **ClientManager.Infrastructure**: Detalhes técnicos, persistência **assíncrona** no RavenDB com suporte a exclusão lógica e extensões.
*   **Tests**: Projetos de testes unitários para cada camada lógica da aplicação.

## 🛠️ Tecnologias e Padrões Aplicados

*   **Framework:** .NET 9.0
*   **Banco de Dados:** RavenDB (NoSQL com suporte a anexos).
*   **Mapeamento:** Mapeamento manual de DTOs para maior performance e controle.
*   **Exclusão Lógica:** Implementação de **Soft Delete** em todas as entidades principais.
*   **Operações Assíncronas:** Uso extensivo de `async/await` e `IAsyncDocumentSession` para alta performance.
*   **Validação de Identidade:** Helpers de domínio para **CPF** e **CNPJ**.
*   **Gestão de Documentos:** Categorização por tipo e controle de **Data de Expiração**.
*   **Segurança de Upload:** Políticas de extensão e tamanho via `IFileValidator`.
*   **Status de Verificação:** Lógica automática para classificar clientes como `Verified`, `Attention` ou `Pending` com base nos documentos.
*   **Suporte Multi-perfil:** Tratamento diferenciado para **Pessoa Física (PF)** e **Pessoa Jurídica (PJ)**.
*   **Mensageria:** Integração com **RabbitMQ (CloudAMQP)** para processamento assíncrono.
*   **Envio de E-mail:** Suporte híbrido para **SendGrid** (Produção) e **SMTP/Mailtrap** (Desenvolvimento).
*   **Documentação:** Swagger e Scalar (Modern API Docs).
*   **Testes:** xUnit, Moq (Mocking) e FluentAssertions.

## 🚀 Como Rodar o Projeto

O projeto está configurado para utilizar **ambientes distintos**:
- **Desenvolvimento (Localhost):** Utiliza **RabbitMQ via Docker** e SMTP local (Mailtrap) para testes.
- **Produção (Deploy):** Utiliza serviços gerenciados na nuvem: **RavenDB Cloud**, **CloudAMQP (RabbitMQ)** e **SendGrid**.

### 1. Pré-requisitos
*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
*   **Docker Desktop** (ou Docker Engine) instalado.
*   Conta no [CloudAMQP](https://www.cloudamqp.com/) (**Apenas para ambiente de Deploy/Produção**).
*   Conta no [SendGrid](https://sendgrid.com/) (Produção) ou [Mailtrap](https://mailtrap.io/) (Desenvolvimento).

### 2. Configuração do Ambiente de Desenvolvimento
1. Na raiz do projeto, suba os containers do **RavenDB** e do **RabbitMQ local**:
```bash
docker-compose up -d
```
*Nota: O RabbitMQ local estará acessível em `localhost:5672`.*

2. Configure as credenciais de desenvolvimento usando **User Secrets** (Recomendado):
```bash
# No diretório src/ClientManager.Worker e src/ClientManager.Api
dotnet user-secrets set "ConnectionStrings:RabbitMQ" "amqp://guest:guest@localhost:5672"
dotnet user-secrets set "Smtp:Username" "seu_usuario_mailtrap"
dotnet user-secrets set "Smtp:Password" "sua_senha_mailtrap"
```

### 3. Configuração do Banco de Dados
1.  Acesse o painel do RavenDB em `http://localhost:8080`.
2.  Crie um novo banco de dados chamado: **`ClientManagementDB`**.

### 4. Execução do Projeto
Você precisará rodar tanto a **API** quanto o **Worker** para o fluxo completo (Cadastro + Envio de E-mail):

**Rodar a API:**
```bash
dotnet run --project src/ClientManager.Api/ClientManager.Api.csproj
```

**Rodar o Worker:**
```bash
dotnet run --project src/ClientManager.Worker/ClientManager.Worker.csproj
```

### 5. Configuração de Produção
No ambiente de produção (`appsettings.json`), o sistema prioriza o **SendGrid** se a seção `Smtp` não estiver presente:
- Configure `SendGrid:ApiKey` e `SendGrid:FromEmail`.
- Configure `ConnectionStrings:RabbitMQ` com a URL do CloudAMQP.

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

