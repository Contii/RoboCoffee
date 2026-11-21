# Plano de commits

## Objetivo

Entregar primeiro os requisitos obrigatórios com commits pequenos, completos e testados.
Os itens do “Vá Além” só serão iniciados após o MVP estar funcional.

Após cada commit aprovado e executado, atualizar este arquivo para marcar a etapa como concluída.
Adicionar novas etapas somente quando houver uma necessidade técnica real identificada durante a implementação.
Não criar etapas para especular sobre funcionalidades sem relação com o andamento do projeto.

Nenhum commit deve deixar a aplicação quebrada. Cada commit precisa compilar, passar nos testes existentes e confirmar manualmente ou por testes o comportamento implementado.

## Antes de cada commit

Executar:

```powershell
dotnet build
dotnet test
```

Para commits que envolvam Docker ou banco:

```powershell
docker compose up --build
dotnet test
```

Verificar também:

- Swagger abre e os endpoints da alteração respondem.
- Migration executa sem alterar dados antigos incorretamente.
- Logs não expõem dados sensíveis.
- Testes cobrem o comportamento obrigatório.
- Nenhum arquivo temporário ou segredo será incluído.

Nenhum comando Git deve ser executado antes de eu revisar o comando e o efeito esperado.

## Fase 1 — Obrigatório

| Etapa | Trabalho | Commit principal |
|---:|---|---|
| 01 | ✅ Criar solution, projetos e estrutura inicial | `chore: initialize solution structure` |
| 02 | ✅ Configurar Swagger, health checks e tratamento global de erros | `feat: configure api infrastructure` |
| 03 | ✅ Modelar `Product`, `Order`, `OrderItem` e enumerações | `feat: add catalog and order domain models` |
| 04 | ✅ Configurar EF Core, DbContext e migrations iniciais | `feat: configure database and initial migration` |
| 05 | ✅ Implementar CRUD de produtos | `feat: add product catalog endpoints` |
| 06 | ✅ Implementar FluentValidation e ProblemDetails | `feat: validate product requests` |
| 07 | ✅ Implementar criação de pedidos e cálculo do total | `feat: add order creation` |
| 08 | ✅ Implementar snapshot do preço e dados do produto no item | `feat: preserve product price in orders` |
| 09 | ✅ Implementar indisponibilidade e quantidade mínima | `fix: enforce order creation rules` |
| 10 | ✅ Implementar transições de situação e cancelamento | `feat: add order status workflow` |
| 11 | ✅ Integrar consulta de endereço ViaCEP | `feat: integrate address lookup with viacep` |
| 12 | ✅ Adicionar timeout, retry e circuit breaker | `feat: add resilient address integration` |
| 13 | Configurar Serilog e correlation ID | `feat: add structured logging` |
| 14 | Adicionar Dockerfile e Docker Compose | `chore: add api and postgres containers` |
| 15 | Adicionar testes unitários das regras | `test: cover order domain rules` |
| 16 | Adicionar testes de integração da API e PostgreSQL | `test: cover api and persistence integration` |
| 17 | Finalizar README obrigatório | `docs: document setup and mandatory features` |

## Commits menores dentro das etapas

Os commits principais podem ser divididos para manter unidades coerentes. Exemplos:

```text
feat: add product entity
feat: configure product ef mapping
feat: add create product endpoint
feat: add list products endpoint
feat: add update product endpoint
feat: add delete product endpoint
test: cover product validation
fix: reject empty product names
```

Exemplos para pedidos:

```text
feat: add order and order item entities
feat: configure order persistence
feat: calculate order total
feat: copy product snapshot to order item
feat: add received to preparing transition
feat: add received order cancellation
test: reject unavailable product
test: reject order without items
test: preserve historical unit price
test: reject invalid status transition
```

Cada commit deve incluir somente o código necessário para a alteração que comunica.
Correções descobertas durante testes devem ser commits separados ou incorporadas antes da entrega da funcionalidade, sem commitar a aplicação quebrada.

## Fase 2 — “Vá Além”

Iniciar apenas após o MVP obrigatório estar estável:

| Ordem | Trabalho | Commit principal |
|---:|---|---|
| 1 | Estoque limitado e controle de concorrência | `feat: prevent overselling limited products` |
| 2 | Idempotency-Key no checkout | `feat: prevent duplicate order submissions` |
| 3 | JWT e políticas por cargo | `feat: add role-based order policies` |
| 4 | Outbox da nota fiscal | `feat: add asynchronous fiscal receipt flow` |
| 5 | Resiliência e observabilidade adicionais | `feat: harden external integration` |
| 6 | GitHub Actions | `ci: add build and test pipeline` |
| 7 | Revisão final de segurança e performance | `chore: harden api and finalize review` |
| 8 | README completo com decisões e limitações | `docs: document architecture trade-offs and ai usage` |

## Quantidade esperada

- MVP obrigatório: aproximadamente 25 a 40 commits.
- MVP com Docker e testes: aproximadamente 35 a 50 commits.
- Com dois itens do “Vá Além”: aproximadamente 45 a 65 commits.

Esses números são uma referência, não uma meta.

## Commits não funcionais

São aceitáveis apenas quando não deixarem a solução em estado quebrado:

- Setup de ferramentas.
- Configuração de CI.
- Documentação que não altera comportamento.
- Ajustes de formatação após build e testes.

Commits como `wip`, `tentativa`, `ajustes`, `finalizando` e commits com código que não compila não são adequados para a entrega principal.
