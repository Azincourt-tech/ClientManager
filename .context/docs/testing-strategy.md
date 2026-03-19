# Testing Strategy

## Frameworks
- **xUnit**: Principal framework para testes unitários.
- **Moq**: Utilizado para criação de mocks e stubs.
- **FluentAssertions**: Utilizado para asserções mais legíveis e expressivas.

## Test Types
- **Unit Tests**: Testam serviços de domínio, modelos e handlers de aplicação de forma isolada.
- **Integration Tests**: Testam componentes de API e Infraestrutura (Repositórios) utilizando instâncias reais ou mocks de RavenDB.

## Execução
Execute os testes via linha de comando:
```bash
dotnet test
```
Ou utilize o **Test Explorer** no Visual Studio / Rider.

