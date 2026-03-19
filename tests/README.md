# ClientManager Tests

Este diretório contém os projetos de testes da aplicação ClientManager. O projeto utiliza uma estratégia de testes piramidal, com foco em testes unitários para garantir a integridade da lógica de negócio.

## Estrutura de Testes

A estrutura de testes espelha a estrutura da aplicação principal (`src/`):

| Projeto de Teste | Camada Alvo | Descrição |
|---|---|---|
| `ClientManager.Api.Tests` | `Api` | Testa os controllers, retornos de status code e fluxo de entrada da API. |
| `ClientManager.Application.Tests` | `Application` | Testa os serviços de aplicação, orquestração e mapeamentos (Mappers). |
| `ClientManager.Domain.Services.Tests` | `Domain.Services` | Testa os serviços de domínio e as regras de negócio complexas. |
| `ClientManager.Domain.Tests` | `Domain` | Testa as entidades de domínio e suas regras internas (Rich Domain Model). |

## Tecnologias Utilizadas

- **xUnit**: Framework principal de execução de testes.
- **Moq**: Biblioteca para criação de objetos simulados (Mocks).
- **FluentAssertions**: Utilizado para asserções mais legíveis e expressivas.
- **Microsoft.AspNetCore.Mvc.Testing**: Suporte para testes de integração de API.

## Como Executar os Testes

Para rodar todos os testes da solução, execute o seguinte comando na raiz do projeto:

```bash
dotnet test
```

Para rodar testes de um projeto específico:

```bash
dotnet test tests/ClientManager.Application.Tests/ClientManager.Application.Tests.csproj
```

### Cobertura de Código

Para gerar relatórios de cobertura de código (caso o `reportgenerator` esteja instalado):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Padrões de Teste

- **AAA (Arrange, Act, Assert)**: Todos os testes seguem o padrão de organizar o cenário, agir e validar o resultado.
- **Nomenclatura**: Os métodos de teste seguem o padrão `NomeDoMetodo_Cenario_ResultadoEsperado`.
  - Exemplo: `CreateCustomer_WhenDataIsValid_ShouldReturnSuccess`.
- **Mocks**: Dependências externas (como Repositórios e Sessions do RavenDB) são simuladas usando Moq para garantir o isolamento dos testes unitários.
