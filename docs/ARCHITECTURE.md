# Arquitetura técnica

## 1. Visão geral

O projeto será uma WebAPI REST para cadastro do cardápio e gerenciamento de pedidos de uma cafeteria.

A solução deve demonstrar:

- Separação clara de responsabilidades.
- Regras de negócio encapsuladas no domínio.
- Persistência com EF Core.
- Integração com API pública.
- Testes unitários e de integração.
- Docker Compose para execução local.
- Documentação OpenAPI.
- Histórico Git organizado.
- Uso transparente de IA.

O foco inicial é entregar os requisitos obrigatórios com alta qualidade. Os itens do "Vá Além" serão implementados somente após o MVP estar estável.

## 2. Arquitetura

Adotar um monólito modular com organização em camadas, com projetos separados por responsabilidade:

```text
RoboCoffee/
├── src/
│   ├── RoboCoffee.Api/
│   ├── RoboCoffee.Application/
│   ├── RoboCoffee.Domain/
│   ├── RoboCoffee.Infrastructure/
│   └── RoboCoffee.Contracts/
└── tests/
    ├── RoboCoffee.UnitTests/
    └── RoboCoffee.IntegrationTests/
```

Dentro da aplicação, organizar os casos de uso por feature:

```text
Application/
├── Catalog/
├── Orders/
├── Integrations/
└── Audit/
```

Essa arquitetura foi escolhida porque:

- O domínio não justifica microsserviços.
- Controllers não devem concentrar regras de negócio.
- Permite evolução futura sem uma reescrita completa.
- Demonstra SOLID, KISS e DRY sem CQRS completo ou abstrações desnecessárias.
- Mantém entidades ricas e casos de uso separados da infraestrutura.

Evitar repository genérico sobre EF Core. O DbContext e as extensões EF Core devem ser usados diretamente pela infraestrutura quando forem suficientes.

## 3. Stack

| Tecnologia | Uso |
|---|---|
| .NET 10 / ASP.NET Core | Plataforma principal |
| Entity Framework Core 10 | ORM e migrations |
| PostgreSQL | Banco principal |
| SQLite | Alternativa opcional apenas para desenvolvimento rápido |
| Docker / Docker Compose | API e banco |
| xUnit | Testes unitários |
| FluentAssertions | Asserções legíveis |
| WebApplicationFactory | Testes de integração da API |
| Testcontainers.PostgreSql | Testes com PostgreSQL real |
| FluentValidation | Validação dos comandos |
| Swagger/OpenAPI | Contrato HTTP |
| Polly | Resiliência HTTP |
| Serilog | Logs estruturados |

Confirmar a versão do SDK antes da implementação:

```powershell
dotnet --version
dotnet new globaljson
```

### Resiliência

Escolher uma única tecnologia para resiliência HTTP.

Para este projeto, Polly será utilizada com:

- Timeout curto.
- Dois retries com backoff exponencial.
- Circuit breaker.
- Logging das falhas.

A API externa não deve comprometer a criação de pedidos quando estiver indisponível.

## 4. Estrutura do domínio

### Product

```text
Product
- Id: Guid
- Name: string
- Category: enum
- Size: enum P, M, G
- Price: decimal
- IsAvailable: bool
- StockQuantity: int?
- IsActive: bool
- CreatedAtUtc
- UpdatedAtUtc
- RowVersion
```

`Category` e `Size` podem ser enums persistidos como texto para facilitar leitura no banco.

### Order

```text
Order
- Id: Guid
- CustomerName: string
- CustomerDocument: string?
- PostalCode: string?
- Status: enum
- Total: decimal
- CreatedAtUtc
- UpdatedAtUtc
- RowVersion
```

O total é calculado pela API e persistido como snapshot do pedido.

### OrderItem

```text
OrderItem
- Id: Guid
- OrderId: Guid
- ProductId: Guid
- ProductNameSnapshot: string
- Quantity: int
- UnitPriceSnapshot: decimal
- Subtotal: decimal
- Observation: string?
```

### ProductPriceHistory

```text
ProductPriceHistory
- Id: Guid
- ProductId: Guid
- OldPrice: decimal
- NewPrice: decimal
- ChangedAtUtc
- ChangedBy: string?
```

Esse histórico é opcional para auditoria e não substitui o snapshot do item.

## 5. Relacionamentos

- `Order` possui vários `OrderItem`.
- `Product` pode estar relacionado a vários `OrderItem`.
- `OrderItem` não pode ser alterado após a criação do pedido.
- `Product` não deve ser fisicamente excluído após participar de pedidos.
- `Product` pode ser desativado com exclusão lógica.

## 6. Regras de domínio

As regras críticas devem existir em entidades e métodos de domínio, não em controllers.

Regras obrigatórias:

- Produto indisponível não pode ser pedido.
- Todo pedido possui pelo menos um item.
- Quantidade deve ser maior que zero.
- Total é calculado pela API.
- O preço unitário do pedido é imutável.
- Situação segue `Received → Preparing → Ready → Delivered`.
- Cancelamento só é permitido no estado `Received`.
- Transições inválidas retornam `409 Conflict`.

Métodos sugeridos:

```text
Pedido.AdicionarItem(...)
Pedido.CalcularTotal()
Pedido.AvancarStatus()
Pedido.Cancelar()
```

Exceções de domínio devem ser traduzidas pela API para `ProblemDetails`.

## 7. Imutabilidade do preço

A regra será garantida em três níveis:

1. Ao criar o pedido, copiar nome e preço atual do produto para o item.
2. Não expor endpoints capazes de alterar itens já criados.
3. Criar os itens e validar o pedido dentro da mesma transação.

Fluxo:

```text
1. Iniciar transação.
2. Buscar produtos.
3. Verificar disponibilidade.
4. Capturar preço atual.
5. Validar quantidade.
6. Calcular total.
7. Criar Order e OrderItems com snapshots.
8. Confirmar a transação.
```

O preço do pedido antigo não deve ser recalculado consultando `Product.Price`.

## 8. Máquina de estados

Usar enumeração e métodos de domínio:

```text
Received.AdvanceToPreparing()
Preparing.AdvanceToReady()
Ready.AdvanceToDelivered()
Received.Cancel()
```

Evitar atribuição direta de status em handlers ou controllers.

## 9. API pública

ViaCEP é a integração inicial recomendada.

Valor para o negócio:

- Preenchimento de endereço pelo CEP.
- Redução de digitação incorreta.
- Confirmação pelo atendente.
- Melhor experiência em pedidos para entrega.

Implementação:

- Interface `IAddressLookupService`.
- `HttpClient` tipado registrado no DI.
- Timeout curto.
- Dois retries com backoff exponencial.
- Circuit breaker.
- Cache em memória por CEP por 24 horas.
- Falha externa não impede o checkout.
- Resposta traduzida para um modelo próprio da aplicação.

Não chamar a ViaCEP diretamente no controller ou no repositório.

## 10. Validação e erros HTTP

Usar:

- FluentValidation para entradas.
- ProblemDetails para respostas.
- Exception handler global.
- Mensagens estáveis e sem detalhes internos sensíveis.

Status sugeridos:

- `400 Bad Request`: payload ou validação inválida.
- `404 Not Found`: recurso inexistente.
- `409 Conflict`: estado inválido, estoque insuficiente ou chave idempotente conflitante.
- `422 Unprocessable Entity`: somente quando houver uma necessidade clara de distinguir validação semântica.
- `500 Internal Server Error`: falhas inesperadas.

## 11. API, DTOs e contratos

- Controllers finos.
- DTOs separados das entidades.
- Casos de uso ou services específicos para operações do domínio.
- Nenhum campo definido pela API pode alterar diretamente `Total`, `Status` ou `UnitPriceSnapshot`.
- IDs podem ser gerados pela API ou pelo domínio, mas nunca recebidos como preço ou estado do pedido.
- Endpoints de listagem devem possuir paginação.

## 12. Persistência

Usar EF Core com PostgreSQL:

- Migrations versionadas.
- Configurações separadas por entidade.
- Índices em campos consultados frequentemente.
- Constraints definidas no banco e validadas no domínio.
- Transações para criação de pedidos.
- Row version para operações concorrentes.

Não aplicar migrations manualmente em produção sem processo documentado.

## 13. Docker

O projeto deve permitir execução por Docker Compose com um único comando:

```yaml
services:
  api:
    build: .
    ports:
      - "8080:8080"
  postgres:
    image: postgres:16
    environment:
      POSTGRES_DB: robocoffee
      POSTGRES_USER: robocoffee
      POSTGRES_PASSWORD: robocoffee
    volumes:
      - postgres-data:/var/lib/postgresql/data

volumes:
  postgres-data:
```

Não armazenar senhas reais no repositório. Usar `.env.example`.

## 14. Logs e rastreabilidade

Serilog será usado para logs estruturados com:

- Correlation ID.
- `OrderId` quando disponível.
- Operação realizada.
- Resultado.
- Exceção sanitizada.

Não registrar dados desnecessários ou dados sensíveis.

OpenTelemetry pode ser uma evolução futura, mas não é necessário no MVP.

## 15. Testes

### Testes unitários

- Produto indisponível não pode ser pedido.
- Pedido vazio não é permitido.
- Quantidade menor que um é rejeitada.
- Total é calculado pela API.
- Alterar preço não altera pedido antigo.
- Cancelamento só é permitido em `Received`.
- Todas as transições inválidas são rejeitadas.

### Testes de integração

Usar `WebApplicationFactory` para:

- Criar pedido pela API.
- Persistir pedido e itens.
- Consultar o pedido completo.
- Validar migrations.
- Simular falha da ViaCEP.
- Validar códigos HTTP.
- Validar swagger e health checks.

Testcontainers pode ser usado para executar PostgreSQL real em integração. Preferir PostgreSQL real ao SQLite para testes concorrentes e de persistência.

## 16. Itens do “Vá Além”

Implementar somente depois do MVP obrigatório estar fechado.

### Pico das 8h

- Campo `StockQuantity`.
- Transação.
- Lock pessimista ou comparação com row version.
- Atualização de estoque e criação do pedido na mesma transação.
- `409 Conflict` para estoque insuficiente.

### Toque duplo

- Exigir `Idempotency-Key`.
- Persistir chave, hash da requisição e resposta.
- Retornar o pedido original para chave e payload iguais.
- Rejeitar chave reutilizada com payload diferente.

### Nota fiscal

- Entrega não deve aguardar o emissor fiscal.
- Usar registro Outbox na mesma transação da entrega.
- Processar com `BackgroundService`.
- Registrar tentativas e falhas.
- Permitir simular emissor lento ou indisponível.

### API externa indisponível

- Marcar endereço como não confirmado.
- Permitir continuar o pedido.
- Informar claramente que o endereço não foi preenchido pela integração.

### Perfis e autorização

- Atendente cria pedidos.
- Barista altera status.
- Gerente altera cardápio e preços.
- Admin administra todos os recursos.

Usar políticas no ASP.NET Core, como:

```text
[Authorize(Policy = "CanManageCatalog")]
```

JWT pode ser demonstrado no futuro, mas não faz parte do núcleo obrigatório.

### Meu pedido sumiu

Usar correlation ID em todas as operações relevantes e logs estruturados com `OrderId`.
Isso permite rastrear criação, atualização de status e falhas sem espalhar dados sensíveis.

## 17. Testes, CI e Docker

Executar localmente:

```powershell
dotnet build
dotnet test
docker compose up --build
```

GitHub Actions pode ser configurado depois dos testes locais. A pipeline mínima deve executar restore, build e testes a cada push ou pull request.
