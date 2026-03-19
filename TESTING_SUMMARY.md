# Resumo de Testes (Testing Summary)

Este documento fornece um panorama geral da estratégia e cobertura de testes do projeto ClientManager.

## Visão Geral

O projeto ClientManager adota uma estratégia de testes focada em **Testes Unitários** para garantir que todas as regras de negócio de domínio e orquestração de aplicação funcionem conforme esperado de forma isolada e previsível.

## Status da Cobertura por Camada

| Camada | Tipo de Teste | Status | Cobertura Aproximada |
|---|---|---|---|
| **Api** | Unit / Integration | ✅ Ativo | Controllers e fluxos de retorno principais. |
| **Application** | Unit | ✅ Ativo | Orquestradores, Mappers e Validações. |
| **Domain Services** | Unit | ✅ Ativo | Regras de negócio cruzadas de domínio. |
| **Domain Model** | Unit | ✅ Ativo | Lógica interna de entidades e Value Objects. |

## Principais Cenários Cobertos

### Clientes (Customer)
- **Validação de Documento**: Garantia de CPFs e CNPJs válidos através do `DocumentHelper`.
- **Ativação e Inativação**: Lógica de ativação e tratamento de e-mails já existentes.
- **Mapeamento**: Transformação correta de DTOs para Modelos e vice-versa.
- **Gestão de Endereço**: Validação e atualização de endereços (Pessoa Física e Jurídica).

### Documentos (Document)
- **Classificação de Status**: Lógica que define se o cliente é `Verified`, `Attention` ou `Pending` baseada na data de expiração e validade dos documentos.
- **Upload e Validação**: Regras de extensão permitida e tamanho máximo de arquivo.
- **Ciclo de Vida**: Exclusão lógica (Soft Delete) e persistência de metadados.

## Pilha Tecnológica de Testes

- **xUnit**: Executor de testes para o ecossistema .NET.
- **Moq**: Simulação de dependências (Injeção de Dependência em testes).
- **FluentAssertions**: Para asserções expressivas e legíveis.
- **TestHost / WebApplicationFactory**: Utilizado para testes de integração na camada de API.

## Boas Práticas Adotadas

1.  **Isolamento**: Cada teste unitário é isolado de fontes externas (Banco de Dados, Redes) via Mocks.
2.  **Reprodutibilidade**: Testes garantem o mesmo resultado independentemente do ambiente de execução.
3.  **Legibilidade**: O uso de `FluentAssertions` e o padrão `AAA` tornam os testes fáceis de entender.
4.  **Integração Contínua (CI)**: O projeto está preparado para rodar testes automaticamente em cada `push` (configurado via GitHub Actions em `.github/workflows/ci.yml`).

## Como Analisar Resultados

Os resultados dos testes podem ser visualizados via terminal executando `dotnet test` ou via **Test Explorer** em IDEs como Visual Studio, JetBrains Rider ou extensões do VS Code.
