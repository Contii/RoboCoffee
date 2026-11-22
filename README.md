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

## Docker

Com Docker Compose:

```powershell
docker compose up --build
```

A API ficará disponível em `http://localhost:8080`, PostgreSQL na porta pública `5433` e o Swagger em `http://localhost:8080/swagger`.

Copie `.env.example` para `.env` para substituir as credenciais locais. O arquivo `.env` não é versionado.

No ambiente `Development`, a API aplica as migrations EF Core automaticamente na inicialização.

## Cobertura de regras

Os testes unitários cobrem produto indisponível ou inativo, pedido vazio, quantidade mínima, snapshot de preço, total, cancelamento e todas as transições inválidas da máquina de estados.

Os testes de integração cobrem API com EF Core InMemory e persistência real PostgreSQL por Testcontainers. Para executar PostgreSQL real, Docker precisa estar disponível:

```powershell
dotnet test tests/RoboCoffee.IntegrationTests/RoboCoffee.IntegrationTests.csproj
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
POST /api/orders/{id}/advance
POST /api/orders/{id}/cancel
```

O checkout cria o pedido, calcula o total a partir dos itens e preserva nome e preço atual dos produtos nos snapshots.

Na criação de pedidos:

- a quantidade deve ser maior que zero;
- pedidos precisam possuir pelo menos um item;
- produtos indisponíveis ou inativos não podem ser pedidos;
- quantidade inválida retorna `400 Bad Request`;
- produto indisponível retorna `409 Conflict`.

Os pedidos avançam por `Received -> Preparing -> Ready -> Delivered`.
O cancelamento só é permitido em `Received`; transições inválidas retornam `409 Conflict`.

## Consulta de endereço

CEP pode ser consultado pelo ViaCEP:

```text
GET /api/addresses/{postalCode}
```

CEP com oito dígitos retorna `200` com os dados confirmados. CEP inválido retorna `400`; CEP inexistente ou falha externa retorna `404`, sem impedir a criação de pedidos.

O cliente ViaCEP usa timeout de 5 segundos, até dois retries com backoff exponencial e circuit breaker após cinco falhas transitórias.

## Logs e rastreabilidade

A API usa Serilog para logs estruturados no console.

Cada requisição aceita o header opcional `X-Correlation-ID`. Quando informado, o mesmo valor é usado no log e devolvido na resposta. Quando ausente ou maior que 128 caracteres, a API gera um GUID.

## Arquitetura

O projeto usa monólito modular em camadas:

- `RoboCoffee.Api`: controllers, middleware, Swagger, health checks e composição;
- `RoboCoffee.Application`: casos de uso, interfaces e validadores;
- `RoboCoffee.Domain`: entidades, invariantes e máquina de estados;
- `RoboCoffee.Infrastructure`: EF Core, PostgreSQL, ViaCEP e configurações externas;
- `RoboCoffee.Contracts`: DTOs de entrada e saída.

Regras críticas não ficam nos controllers. EF Core é usado diretamente por services específicos; o projeto não usa repository genérico, CQRS ou Event Sourcing.

Decisões técnicas e limites estão documentados em [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Testes

```powershell
dotnet test
```

A solução possui testes unitários e de integração:

- regras de domínio;
- criação e persistência de produtos;
- criação e persistência de pedidos;
- snapshots e cálculo do total;
- estados e cancelamento;
- FluentValidation e ProblemDetails;
- health, Swagger e correlation ID;
- ViaCEP com respostas simuladas;
- PostgreSQL real por Testcontainers.

Testcontainers exige Docker disponível.

## Funcionalidades obrigatórias concluídas

- solution .NET 10 organizada por responsabilidade;
- Swagger, OpenAPI e health check;
- entidades rich domain para catálogo e pedidos;
- EF Core com PostgreSQL e migrations;
- CRUD de produtos com soft delete;
- FluentValidation e respostas ProblemDetails;
- criação de pedidos e cálculo do total;
- snapshots históricos de nome e preço;
- validação de disponibilidade e quantidade;
- fluxo de status e cancelamento;
- consulta ViaCEP com fallback;
- timeout, retry e circuit breaker;
- Serilog e X-Correlation-ID;
- Dockerfile e Docker Compose;
- testes unitários e de integração;
- documentação técnica e ensaio privado.

## Limites atuais

Os itens abaixo pertencem ao plano “Vá Além” e ainda não fazem parte do MVP:

- controle de estoque limitado e concorrência;
- Idempotency-Key;
- JWT e políticas por cargo;
- Outbox para emissão fiscal;
- cache por CEP;
- GitHub Actions;
- revisão final de segurança e performance.
