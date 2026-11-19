# RoboCoffee
Sistema back-end para automação do ciclo de vida de pedidos, do balcão à entrega, com integração a APIs externas.

## Estado atual

A solution .NET 10 foi criada com projetos separados por responsabilidade:

- `RoboCoffee.Api`
- `RoboCoffee.Application`
- `RoboCoffee.Domain`
- `RoboCoffee.Infrastructure`
- `RoboCoffee.Contracts`
- `RoboCoffee.UnitTests`
- `RoboCoffee.IntegrationTests`

## Desenvolvimento local

Pré-requisito: SDK .NET 10.

```powershell
dotnet restore
dotnet build
dotnet test
```

## API

Durante o desenvolvimento:

```powershell
dotnet run --project src/RoboCoffee.Api/RoboCoffee.Api.csproj --launch-profile http
```

- Swagger UI: `http://localhost:5128/swagger`
- OpenAPI: `http://localhost:5128/openapi/v1.json`
- Health check: `http://localhost:5128/health`
