# Diário de desenvolvimento com IA

Este diário reúne as principais fases do planejamento e desenvolvimento do
RoboCoffee. Ele não é um registro integral das conversas: destaca objetivos,
sugestões da IA, decisões minhas e correções relevantes.

## 001 - Planejamento e direção técnica

Comecei o projeto apresentando o PDF do desafio e defini que a entrega seria uma
API REST em .NET com qualidade técnica prática. Eu não queria uma arquitetura
excessivamente complexa nem microsserviços para um domínio que não justificava
essa divisão.

A IA sugeriu organizar o projeto como monólito modular em camadas, com API,
Application, Domain, Infrastructure e Contracts. Também sugeriu EF Core,
PostgreSQL, Docker, FluentValidation, Polly, Serilog, WebApplicationFactory,
Testcontainers e ViaCEP.

Adotei essa direção. Decidi priorizar o MVP obrigatório antes dos itens
“Vá Além” e separar a documentação entre README, arquitetura, plano de commits,
diário e estudo privado de entrevistas.

Também defini uma regra permanente: nenhum comando Git seria executado sem
minha aprovação explícita. Eu queria ver o comando e o efeito esperado antes
do staging, commit ou push.

## 002 - Processo de documentação e commits

Solicitei que planejamento privado e documentos públicos tivessem funções
claras. `README.md` ficaria voltado ao estado real do projeto,
`docs/ARCHITECTURE.md` registraria decisões duráveis,
`docs/COMMIT_PLAN.md` manteria o backlog e `docs/AI_JOURNAL.md` resumiria as
interações relevantes.

A IA sugeriu um fluxo previsível: consultar o backlog, implementar uma unidade
pequena, testar, revisar o diff, atualizar documentação diretamente relacionada
e apresentar o commit antes de executá-lo.

Adotei esse processo e determinei que commits precisariam compilar e passar nos
testes existentes. Commits quebrados ou com mensagens vagas não seriam aceitos.

## 003 - Estrutura, domínio e persistência

Comecei a implementação pela estrutura da solution e pelos modelos principais.
Autorizei a criação dos projetos e mantive referências coerentes entre API,
Application, Domain, Infrastructure e Contracts.

A IA implementou as entidades `Product`, `Order` e `OrderItem`, enumerações,
exceções de domínio, snapshot de nome e preço, cálculo de total e fluxo
`Received -> Preparing -> Ready -> Delivered`.

Decidi manter regras críticas no domínio. Em seguida, aprovei EF Core com
PostgreSQL, mapeamentos separados por entidade, migration inicial, constraints,
índices e RowVersion.

Eu pedi correções de compatibilidade no SDK, nas versões de Swagger e dos
pacotes ASP.NET Core e revisei os testes antes dos commits iniciais.

## 004 - CRUD, validação e pedidos

Na sequência, implementamos o catálogo de produtos: contratos, service,
persistência EF, controller e operações REST. Produtos desativados usam
soft delete e deixam de aparecer nas consultas normais.

A IA sugeriu FluentValidation para validar nome, categoria, tamanho, preço,
estoque e campos dos pedidos antes da persistência. Mantive validação no
domínio como segunda defesa e configurei `ProblemDetails` para erros HTTP.

Depois implementei pedidos com snapshot de nome e preço, quantidade mínima,
produto ativo e disponível, subtotal e total.

Uma correção importante foi feita no ensaio do projeto: a IA registrou
FluentValidation como já implementada no commit do CRUD, mas eu pedi que o
histórico fosse corrigido. A validação pertence ao commit seguinte.

## 005 - Snapshot e máquina de estados

Mantive a regra de que pedidos antigos não são recalculados quando o catálogo
muda. O nome e o preço são copiados no momento da criação do item.

A IA implementou endpoints para avançar status e cancelar pedidos. Adotei as
transições `Received -> Preparing -> Ready -> Delivered` e permiti cancelamento
somente em `Received`.

Ampliamos os testes para estados inválidos, cancelamento fora de `Received`,
produto inativo e inclusão de itens depois que o pedido avança.

## 006 - ViaCEP, resiliência e observabilidade

Decidi integrar ViaCEP sem acoplar controller ou repositório diretamente ao
serviço externo. A IA criou uma interface própria, modelo próprio e cliente
tipado.

Solicitei que falha externa não impedisse o restante da API. A integração
retorna endereço confirmado quando disponível e fallback `null` para CEP
inválido, inexistente ou indisponível.

A IA sugeriu timeout, dois retries com backoff exponencial e circuit breaker.
Adotei essas políticas para ViaCEP. Elas não incluem cache por CEP nesta etapa.

Também adicionei Serilog e o header `X-Correlation-ID`. Valores informados pelo
cliente são preservados até 128 caracteres; valores ausentes ou excessivos
geram GUID.

## 007 - Docker e persistência real

Autorizei Dockerfile multi-stage e Docker Compose para API e PostgreSQL.
Configurei PostgreSQL em `5432` dentro da rede Docker e em `5433` no host para
evitar conflito com uma instancia local.

A IA sugeriu aplicar migrations na inicialização apenas em Development. Mantive
essa decisão para o ambiente local e preservei migrations controladas em
Production.

Validei a imagem, health, Swagger e criação real de produto pelo PostgreSQL.
Depois implementei testes PostgreSQL com Testcontainers. Mantive testes InMemory
para cenários rápidos e PostgreSQL real para migrations e persistência.

## 008 - Cobertura final e entrega

Na fase final, pedi uma ampliação dos testes unitários das regras obrigatórias
e uma revisão completa do projeto. A IA encontrou lacunas de documentação e
duplicações preventivas.

Eu confirmei o que deveria permanecer como MVP e o que pertence ao futuro:

- não existe cache ViaCEP por 24 horas;
- não existe estoque limitado;
- não existe Idempotency-Key;
- não existe JWT e politicas por cargo;
- não existe Outbox fiscal;
- não existe GitHub Actions;
- não existe associação obrigatoria de `OrderId` aos logs.

Antes da entrega, executei build e testes completos. O estado final teve build
sem avisos ou erros e 28 testes unitários e 28 testes de integração aprovados.

## 009 - Decisão sobre autoria e uso da IA

Muitas decisões técnicas importantes passaram pela minha aprovação, mas eu não
decidi nem revisei tudo isoladamente. Usei a IA como executora e conselheira:
ela sugeriu soluções, implementou alterações, encontrou incompatibilidades,
preparou testes e destacou limites.

Eu determinei objetivos, prioridades e fluxo, pedi correções, autorizei commits
e confirmei documentação e resultados. O projeto representa trabalho meu com
assistência técnica da IA, não trabalho integralmente autônomo nem atribuído
integralmente à IA.

## 010 - Correção do host de testes de infraestrutura

**Contexto**

Na revisão final antes da entrega, os testes de health, Swagger e OpenAPI falharam
porque iniciavam a API em Development e tentavam aplicar migrations contra
PostgreSQL local, que não estava disponível nesse contexto.

**Contribuição da IA**

A IA ajustou o host `ApiInfrastructureTests` para substituir o provider Npgsql
por EF Core InMemory. Os testes de PostgreSQL real continuam usando
Testcontainers separadamente.

**Decisão confirmada**

- Testes de infraestrutura HTTP não dependem de PostgreSQL local.
- Testcontainers permanece responsável pelos testes PostgreSQL reais.
- O startup automático de migrations em Development continua válido.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 28 aprovados.
- Testes de integração: 28 aprovados.

**Correção adicional da isolamento dos pedidos**

Durante a adição dos testes GET de pedidos, o banco InMemory recebeu um nome
diferente por contexto e algumas requisições passaram a usar provedores
isolados incorretamente. Corrigi o fixture para manter um único nome de banco
por instância do teste, garantindo que produto, pedido e consulta compartilhem
os mesmos dados.

**Resultado final da revisão**

- testes unitários: 28 aprovados;
- testes de integração: 32 aprovados;
- build: sucesso, 0 warnings, 0 errors.

## 011 - Consultas completas de pedidos

**Contexto**

Antes da entrega, pedi a adição de consultas HTTP para pedidos por ID e por paginação.

**Contribuição da IA**

A IA adicionou `GetByIdAsync` e `GetPagedAsync` ao service, endpoints GET no
`OrdersController`, inclusão dos itens e testes de integração para sucesso,
404 e paginação.

**Decisões confirmadas**

- `GET /api/orders/{id}` retorna o pedido completo.
- `GET /api/orders?page=1&pageSize=20` retorna pedidos paginados.
- Pedidos são ordenados pelos mais recentes.
- Paginação usa os mesmos limites do catálogo: página maior que zero e tamanho
  entre 1 e 100.
- Esta consulta elimina a lacuna anterior de leitura HTTP registrada no README.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 28 aprovados.
- Testes de integração: 32 aprovados.
