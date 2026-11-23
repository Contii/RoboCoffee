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

O projeto usa monólito modular em camadas:

- `RoboCoffee.Api`: controllers, middleware, Swagger, health checks e composição;
- `RoboCoffee.Application`: casos de uso, interfaces e validadores;
- `RoboCoffee.Domain`: entidades, invariantes e máquina de estados;
- `RoboCoffee.Infrastructure`: EF Core, PostgreSQL, ViaCEP e configurações externas;
- `RoboCoffee.Contracts`: DTOs de entrada e saída.

Regras críticas ficam no domínio. Controllers são finos e EF Core é usado diretamente
por services específicos. O projeto não usa repository genérico, CQRS ou Event Sourcing.

## Domínio atual

O núcleo de domínio já contém os modelos principais:

- `Product`, `ProductCategory` e `ProductSize`
- `Order`, `OrderItem` e `OrderStatus`
- `DomainException`

Os pedidos preservam nome e preço do produto no momento da inclusão do item. A situação é avançada por métodos de domínio e segue o fluxo `Received -> Preparing -> Ready -> Delivered`, com cancelamento permitido apenas em `Received`.

## Desenvolvimento local

Pré-requisitos gerais:

- SDK .NET 10;
- Docker Desktop para Compose e testes PostgreSQL com Testcontainers;
- PostgreSQL em `localhost:5432` quando a API for executada diretamente pelo .NET.

O ambiente da API é definido por `ASPNETCORE_ENVIRONMENT`.

- `Development`: Swagger/OpenAPI habilitados e migrations aplicadas na inicialização;
- `Production`: Swagger/OpenAPI desabilitados e migrations controladas externamente.

<details>
<summary><strong>Opção 1: Docker Compose</strong></summary>

Esta é a forma recomendada para um clone novo.

#### Development

Nenhum `.env` é necessário. Na raiz do repositório:

```powershell
docker compose up --build
```

O Compose cria PostgreSQL 16 e a API em Development. A API aplica migrations
automaticamente na inicialização.

URLs:

- API: `http://localhost:8080`;
- Swagger: `http://localhost:8080/swagger`;
- OpenAPI: `http://localhost:8080/openapi/v1.json`;
- Health: `http://localhost:8080/health`;
- PostgreSQL no host: `localhost:5433`.

Comandos úteis:

```powershell
docker compose ps
docker compose logs api
docker compose logs -f api
docker compose up --build -d
docker compose down
```

Use `docker compose logs api` para ler os logs acumulados e
`docker compose logs -f api` para acompanhar novos logs.
Os logs podem ser filtrados por correlation ID:

```powershell
docker compose logs api | Select-String "CORRELATION-ID"
```

Serilog escreve no stdout/stderr do container. Não há gravação em arquivo nem
envio para serviço externo.

`docker compose down` preserva o volume PostgreSQL. Use
`docker compose down -v` somente para apagar os dados locais.

Para personalizar credenciais ou ambiente, copie o modelo:

```powershell
Copy-Item .env.example .env
```

Edite o `.env`. O arquivo `.env.example` é versionado como referência e
`.env` é ignorado pelo Git.

#### Production no Docker

Defina Production no `.env`:

```text
ASPNETCORE_ENVIRONMENT=Production
POSTGRES_DB=robocoffee
POSTGRES_USER=robocoffee
POSTGRES_PASSWORD=senha-segura
```

Inicie apenas PostgreSQL:

```powershell
docker compose up -d postgres
```

Em outro terminal, configure o ambiente de migration manual com a porta pública
do PostgreSQL:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__PostgreSQL="Host=localhost;Port=5433;Database=robocoffee;Username=robocoffee;Password=senha-segura"
dotnet tool run dotnet-ef database update --project src/RoboCoffee.Infrastructure/RoboCoffee.Infrastructure.csproj --startup-project src/RoboCoffee.Api/RoboCoffee.Api.csproj --context RoboCoffeeDbContext
```

Depois inicie ou reconstrua a API:

```powershell
docker compose up --build -d api
```

Em Production:

- migrations não são aplicadas pela API;
- Swagger/OpenAPI não são habilitados;
- health pode ser consultado em `http://localhost:8080/health`;
- PostgreSQL continua persistindo dados no volume.
- Serilog escreve no stdout/stderr do container;
- use `docker compose logs api` para leitura acumulada;
- use `docker compose logs -f api` para acompanhamento em tempo real;
- use `docker compose down` para preservar dados e `docker compose down -v` para apagá-los.

#### Testes dentro da opção Docker

Os testes não usam os containers da aplicação. Eles usam o projeto local e
Testcontainers separadamente.

```powershell
dotnet test
dotnet test tests/RoboCoffee.UnitTests/RoboCoffee.UnitTests.csproj
dotnet test tests/RoboCoffee.IntegrationTests/RoboCoffee.IntegrationTests.csproj
```

Docker Desktop deve estar disponível para os testes que usam PostgreSQL real.

</details>

<details>
<summary><strong>Opção 2: executar diretamente com .NET</strong></summary>

Esta opção exige PostgreSQL em `localhost:5432`.

#### Development

Configure o User Secret da API:

```powershell
dotnet user-secrets init --project src/RoboCoffee.Api/RoboCoffee.Api.csproj
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=robocoffee;Username=USUARIO;Password=SENHA" --project src/RoboCoffee.Api/RoboCoffee.Api.csproj
```

Restaure e compile a solution:

```powershell
dotnet restore
dotnet build
```

O launch profile `http` define `ASPNETCORE_ENVIRONMENT=Development`. Execute:

```powershell
dotnet run --project src/RoboCoffee.Api/RoboCoffee.Api.csproj --launch-profile http
```

Os logs aparecem no terminal que executou o comando.
Serilog não grava em arquivo nem envia para serviço externo.

URLs:

- Swagger: `http://localhost:5128/swagger`;
- OpenAPI: `http://localhost:5128/openapi/v1.json`;
- Health: `http://localhost:5128/health`.

Nesta opção, a API aplica migrations automaticamente na inicialização.
Dados permanecem no PostgreSQL local entre execuções.

#### Production

Configure ambiente e connection string no processo que iniciará a API:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:ConnectionStrings__PostgreSQL="Host=localhost;Port=5432;Database=robocoffee;Username=USUARIO;Password=SENHA"
```

Aplique migrations manualmente antes da API:

```powershell
dotnet tool run dotnet-ef database update --project src/RoboCoffee.Infrastructure/RoboCoffee.Infrastructure.csproj --startup-project src/RoboCoffee.Api/RoboCoffee.Api.csproj --context RoboCoffeeDbContext
```

Inicie sem o launch profile para não forçar Development:

```powershell
dotnet run --project src/RoboCoffee.Api/RoboCoffee.Api.csproj --no-launch-profile --urls "http://localhost:5128"
```

Os logs continuam aparecendo no terminal que iniciou o processo.

Em Production não haverá Swagger/OpenAPI. Health estará em
`http://localhost:5128/health`.

Serilog escreve no terminal que iniciou o processo. Não há gravação em arquivo
nem envio para serviço externo. Os logs são mantidos apenas pelo terminal
enquanto o processo está disponível.

#### Testes dentro da opção .NET

```powershell
dotnet test
dotnet test tests/RoboCoffee.UnitTests/RoboCoffee.UnitTests.csproj
dotnet test tests/RoboCoffee.IntegrationTests/RoboCoffee.IntegrationTests.csproj
```

Os testes PostgreSQL também precisam de Docker via Testcontainers.

</details>

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
GET  /api/orders?page=1&pageSize=20
GET  /api/orders/{id}
POST /api/orders/{id}/advance
POST /api/orders/{id}/cancel
```

`GET /api/orders/{id}` retorna o pedido completo com seus itens.
`GET /api/orders` retorna pedidos paginados, ordenados pelos mais recentes,
com `page`, `pageSize`, `totalItems`, `totalPages` e `items`.

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

## Decisões técnicas pendentes

Os pontos abaixo estão definidos no projeto ou documentados como evolução, mas
devem ser escolhidos explicitamente em uma próxima entrega:

1. **Cache de ViaCEP** não está implementado. O estado atual consulta o serviço
   em cada lookup e usa timeout, retries e circuit breaker. Cache por CEP por
   24 horas pode ser definido como evolução futura.
2. `Product.UpdatePrice` e `Product.SetAvailability` são métodos de domínio
   existentes e cobertos por testes. O CRUD atual usa `Product.UpdateDetails`
   para atualizar os dados completos. Os métodos podem ser removidos ou
   associados a endpoints específicos sem alterar o comportamento atual.
3. A integração ViaCEP não registra callbacks de Polly para retry ou abertura
   do circuit breaker. Serilog registra a requisição, mas não os eventos internos
   dessa resiliência. Logs dedicados podem ser adicionados como evolução.
4. `StockQuantity` permanece no modelo de produto como preparação para estoque
   limitado. Ele é validado e persistido, mas não possui atualização concorrente
   nem bloqueia overselling. A implementação completa pertence ao item
   “Pico das 8h”.
5. Para demonstração, Docker Compose é a forma recomendada: cria API,
   PostgreSQL, migration e volume local. PostgreSQL local também é suportado
   pela connection string, mas exige uma instância acessível em `5432`.
