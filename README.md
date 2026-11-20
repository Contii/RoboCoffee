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

## Domínio atual

O núcleo de domínio já contém os modelos principais:

- `Product`, `ProductCategory` e `ProductSize`
- `Order`, `OrderItem` e `OrderStatus`
- `DomainException`

Os pedidos preservam nome e preço do produto no momento da inclusão do item. A situação é avançada por métodos de domínio e segue o fluxo `Received -> Preparing -> Ready -> Delivered`, com cancelamento permitido apenas em `Received`.

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

## Persistência

A infraestrutura usa Entity Framework Core com PostgreSQL.

Comandos locais de migrations:

```powershell
dotnet tool run dotnet-ef migrations list --project src/RoboCoffee.Infrastructure/RoboCoffee.Infrastructure.csproj --startup-project src/RoboCoffee.Api/RoboCoffee.Api.csproj --context RoboCoffeeDbContext
dotnet tool run dotnet-ef database update --project src/RoboCoffee.Infrastructure/RoboCoffee.Infrastructure.csproj --startup-project src/RoboCoffee.Api/RoboCoffee.Api.csproj --context RoboCoffeeDbContext
```

No estado atual, `database update` exige uma instância PostgreSQL acessível por `ConnectionStrings:PostgreSQL`.

## Produtos

Endpoints disponíveis:

```text
POST   /api/products
GET    /api/products?page=1&pageSize=20
GET    /api/products/{id}
PUT    /api/products/{id}
DELETE /api/products/{id}
```

`DELETE` usa exclusão lógica. Produtos desativados deixam de aparecer nas consultas.

As requisições de criação e atualização passam por FluentValidation.
Payloads inválidos retornam `400 Bad Request` com `ProblemDetails` e uma coleção `errors` por campo.

## Pedidos

Endpoints disponíveis:

```text
POST /api/orders
```

O checkout cria o pedido, calcula o total a partir dos itens e preserva nome e preço atual dos produtos nos snapshots.

Na criação de pedidos:

- a quantidade deve ser maior que zero;
- pedidos precisam possuir pelo menos um item;
- produtos indisponíveis ou inativos não podem ser pedidos;
- quantidade inválida retorna `400 Bad Request`;
- produto indisponível retorna `409 Conflict`.
