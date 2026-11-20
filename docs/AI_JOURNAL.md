# Diário de desenvolvimento com IA

Este diário reúne as discussões e decisões relevantes realizadas durante o planejamento e o desenvolvimento do projeto. As entradas são temáticas e não representam uma cópia integral das conversas.

## 001 - Apresentação do projeto e objetivo do desafio

**Contexto**

Apresentei o PDF do desafio e defini que o objetivo era construir uma WebAPI REST em .NET com alta qualidade técnica. Também expliquei que buscava uma solução pragmática, sem over-engineering.

Solicitei um plano completo para arquitetura, stack, modelagem, backlog de commits, README e registro das interações com IA.

**Contribuição da IA**

A IA sugeriu um monólito modular organizado em projetos por responsabilidade, um plano em fases, as entidades principais do domínio e uma estrutura inicial de documentação.

**Decisões confirmadas**

- Adotei o monólito modular como arquitetura principal.
- Priorizarei o MVP obrigatório antes dos itens “Vá Além”.
- README, arquitetura, plano de commits e diário terão finalidades separadas.

**Resultado**

Preservei a análise inicial na pasta privada de planejamento para futuras revisões.

## 002 - Planejamento privado e preservação das propostas

**Contexto**

Solicitei que o PDF e as propostas extensas fossem armazenados fora do repositório rastreável, para permitir leitura e revisão posterior.

**Contribuição da IA**

A IA criou uma pasta privada e registrou nela o plano arquitetural e o PDF do desafio.

**Minhas intervenções**

Pedi que o conteúdo markdown fosse reproduzido literalmente, sem reformulação pela IA.

Também deixei claro que nenhum comando Git seria autorizado automaticamente: todo comando precisa ser mostrado com seu efeito antes da execução.

**Decisões confirmadas**

- Os arquivos privados de planejamento ficarão em `.context/`, excluídos pelo `.gitignore`.
- O PDF do desafio não será enviado ao Git.
- Nenhum comando Git será executado sem minha revisão e aprovação.

**Resultado**

`.context/ARCHITECTURE_PLAN.md` passou a ser uma referência privada inicial e o PDF ficou localizado nessa pasta.

## 003 - Revisão da análise do avaliador

**Contexto**

Apresentei uma análise técnica feita em outra ferramenta sobre as expectativas de um avaliador para diferentes níveis de candidatura.

**Contribuição da IA**

A IA comparou os planos e confirmou a compatibilidade entre monólito modular, Clean Architecture pragmática, EF Core, PostgreSQL, Docker, testes e resiliência.

A IA sugeriu:

- incluir `WebApplicationFactory` nos testes de integração;
- priorizar `409 Conflict` para conflitos e estados inválidos;
- usar um único mecanismo de resiliência HTTP;
- tratar JWT e autorização como itens futuros;
- manter CI fora do núcleo inicial;
- usar ViaCEP pela utilidade direta no pedido.

**Decisões confirmadas**

- Mantive o MVP obrigatório antes do “Vá Além”.
- Usarei Testcontainers depois que PostgreSQL estiver estável.
- Usarei Polly para resiliência HTTP.
- Usarei Serilog para rastreabilidade.

**Alterações descartadas**

Não adotei estimativas de prazo ou quantidade de commits como meta. Os números do plano continuam como referência apenas.

## 004 - Regra de commits funcionais

**Contexto**

Informei à IA que já havia acumulado muitos commits quebrados em projetos anteriores e pedi que evitássemos esse problema aqui.

**Contribuição da IA**

A IA incorporou a regra de que cada commit deve representar uma unidade compilável e testável.

**Decisões confirmadas**

- Executarei o build e os testes relevantes antes de apresentar um commit.
- Não criarei commits `wip`, quebrados ou apenas de ajustes.
- A IA revisará os arquivos alterados antes da proposta.
- Nenhuma operação mutável de Git será executada sem minha aprovação.

**Resultado**

Passei a aplicar essa regra no plano de commits e no `AGENTS.md`.

## 005 - Divisão da documentação

**Contexto**

Solicitei uma separação adequada entre planejamento privado, documentação técnica compartilhável e README voltado ao avaliador humano.

**Contribuição da IA**

A IA propôs esta estrutura:

- `README.md`: visão geral do projeto e estado real;
- `docs/ARCHITECTURE.md`: decisões técnicas duráveis;
- `docs/COMMIT_PLAN.md`: backlog e ordem dos commits;
- `docs/AI_JOURNAL.md`: registro transparente das sessões relevantes;
- `.context/`: planejamento privado e referências temporárias.

**Minhas intervenções**

Queria um README normal de repositório, não uma reprodução do PDF ou do planejamento interno.

**Decisões confirmadas**

- O README inicial será curto e evoluirá conforme recursos forem implementados.
- Arquitetura e backlog serão publicados em `docs/`.
- O diário não conterá um transcript integral das conversas.

**Resultado**

A estrutura pública ficou definida e os documentos públicos foram criados no repositório.

## 006 - Convenção operacional do repositório

**Contexto**

Solicitei que implementações, testes, revisões de commit e atualizações documentais acontecessem em um fluxo harmonioso e previsível.

**Decisões confirmadas**

O fluxo acordado foi:

1. Consultar o próximo passo em `docs/COMMIT_PLAN.md`.
2. Implementar a menor unidade completa.
3. Executar build e testes relevantes.
4. Revisar o diff e explicar os arquivos alterados.
5. Atualizar README, arquitetura ou plano somente quando houver informação diretamente relacionada.
6. Apresentar o commit semântico e os comandos Git com seus efeitos.
7. Executar Git somente após minha aprovação explícita.

Também não haverá push sem solicitação explícita.

**Contribuição da IA**

A IA registrou esse fluxo em `AGENTS.md` e nas políticas de commit.

**Resultado**

Adotei esse fluxo como uma regra permanente do projeto.

## 007 - Atualização contínua da documentação

**Contexto**

Defini quais documentos deveriam acompanhar a implementação e a frequência esperada de cada atualização.

**Decisões confirmadas**

- Atualizarei `README.md` conforme funcionalidades reais forem concluídas.
- Atualizarei `docs/ARCHITECTURE.md` somente quando houver decisão arquitetural durável.
- Atualizarei `docs/COMMIT_PLAN.md` ao concluir etapas aprovadas e somente para novas necessidades técnicas reais.
- Atualizarei `docs/AI_JOURNAL.md` com resumos de sessões relevantes.

**Contribuição da IA**

A IA deve gerar resumos a partir do contexto disponível e do estado do repositório, sem exigir que eu copie inputs ou outputs.

**Limitação**

Quando uma informação não puder ser confirmada, será omitida ou marcada como inferida ou não verificada.

## 008 - Estrutura e propósito do diário

**Contexto**

Perguntei como as interações seriam registradas no diário e se precisaria lembrar a IA de atualizá-lo.

**Contribuição da IA**

A IA explicou que prepararia resumos ao concluir sessões relevantes, sem cópia manual por minha parte.

**Minhas intervenções**

Decidi que o diário não usará datas. Usarei entradas numeradas no formato `001 - Título`.

**Decisões confirmadas**

- O diário será agrupado por assunto, não por mensagem isolada.
- A atualização ocorrerá ao concluir uma implementação ou decisão relevante.
- Interações triviais não precisam criar entradas.
- Cada entrada identificará origem, decisor e resultado.

**Resultado**

Adotei a convenção numerada e temática como formato do diário.

## 009 - Refinamento da autoria e dos rótulos

**Contexto**

Encontrei ambiguidade em rótulos como “Decisões adotadas”, “Erros ou rejeições” e “Revisão realizada”, pois não ficava claro quem havia feito cada sugestão ou decisão.

**Minhas intervenções**

Pedi que o diário identificasse explicitamente a autoria e o resultado de cada intervenção.

**Contribuição da IA**

A IA passou a usar uma estrutura mais precisa baseada em contexto, contribuição da IA, minhas intervenções, decisões confirmadas, sugestões descartadas e resultado.

**Decisão confirmada**

O diário não apresentará revisões, erros ou decisões como fatos neutros sem atribuir origem ou responsabilidade.

## 010 - Conflito de edição do diário no VS Code

**Contexto**

Durante a criação anterior do diário, o VS Code apresentou conflitos de alteração em `docs/AI_JOURNAL.md`.

**Diagnóstico da IA**

A IA explicou que alterações obsoletas do painel de edições apontavam para versões antigas ou para um arquivo já removido.

**Ações realizadas**

- Criamos uma cópia temporária como tentativa de isolar o conflito.
- Removi o diário original e o recriei a partir da cópia temporária.
- Não confirmei nenhuma alteração no Git.

**Minha decisão**

Resolvi descartar o estado obsoleto do editor e reconstruir o arquivo posteriormente do zero.

**Resultado**

Removi `docs/AI_JOURNAL.md` intencionalmente e descartei o estado obsoleto do repositório e do Git.

**Regra permanente**

Não restaurarei stale editor state nem copiarei alterações antigas que não existam no disco.

## 011 - Handoff para uma nova sessão

**Contexto**

Queria preservar o contexto sem usar `Clear Chat` ou apagar a conversa atual.

**Contribuição da IA**

A IA criou `.context/SESSION_HANDOFF.md` com estado do projeto, decisões permanentes, estrutura dos documentos, próxima etapa do plano e regras de aprovação para Git.

**Decisões confirmadas**

- O handoff será lido no início da próxima sessão.
- O diário removido não será restaurado a partir de stale editor state.
- A nova sessão poderá ser iniciada pelo handoff e pela documentação pública.

**Resultado**

Preparei o projeto para continuar sem depender da recuperação do arquivo excluído.

## 012 - Reconstrução deste diário

**Contexto**

Nesta nova sessão, pedi explicitamente que `docs/AI_JOURNAL.md` fosse criado novamente com entradas temáticas numeradas.

**Contribuição da IA**

A IA leu os documentos obrigatórios, o handoff e o histórico local da sessão anterior.

**Decisões confirmadas**

- A reconstrução será baseada em evidências locais e no handoff.
- Informações não verificadas serão omitidas ou marcadas.
- Não restaurarei alterações obsoletas do painel do editor.
- O formato identificará claramente as propostas da IA e minhas intervenções.

**Resultado**

Criei este arquivo como uma reconstrução temática e concisa do planejamento realizado antes da implementação.

## 013 - Início da implementação da estrutura inicial

**Contexto**

Nesta sessão, pedi que o próximo passo fosse determinado pelo handoff e autorizei a preparação da Etapa 1 do plano de commits.

**Contribuição da IA**

A IA confirmou que o SDK .NET não estava disponível no PATH ativo, localizou o SDK 10.0.404 no winget e iniciou a estrutura da solution após minha aprovação.

**Decisões confirmadas**

- SDK .NET 10.0.404 confirmado como versão ativa.
- Criei uma solution `.slnx` com API, Application, Domain, Infrastructure, Contracts e dois projetos de testes.
- Mantive referências iniciais coerentes com a separação de camadas.
- Removi o exemplo `WeatherForecast` e os arquivos placeholder dos templates.
- Atualizei o README com o estado real da estrutura e os comandos locais.

**Minha aprovação**

Autorizei a criação da estrutura inicial antes da implementação das regras de negócio.

**Revisões realizadas**

- `dotnet build RoboCoffee.slnx`: sucesso, com zero avisos e zero erros.
- `dotnet test RoboCoffee.slnx --no-restore`: executado sem testes disponíveis nos projetos xUnit.
- A restauração dos projetos foi concluída com sucesso.
- Identifiquei que `.context/` e `*.pdf` não estavam explícitos no `.gitignore`; restaurei essas regras.
- Removi o PDF duplicado da raiz, preservando somente a cópia em `.context/`.

**Resultado**

A Etapa 1 está pronta como uma unidade de estrutura compilável, aguardando revisão do diff e aprovação do commit.

**Commit relacionado**

`4c9d569` - `chore: initialize solution structure`

## 014 - Configuração da infraestrutura da API

**Contexto**

Implementei a Etapa 2 do plano de commits para disponibilizar documentação HTTP, health check e tratamento sanitizado de erros inesperados.

**Contribuição da IA**

A IA configurou Swagger UI, OpenAPI, health checks e um `IExceptionHandler` que retorna `ProblemDetails` com status 500 sem incluir detalhes internos da exceção.

**Decisões confirmadas**

- Mantive o OpenAPI nativo junto do Swagger UI.
- `/health` responde `200 OK` com o texto `Healthy`.
- Falhas inesperadas retornam um `ProblemDetails` sanitizado.
- Os testes usam `WebApplicationFactory<Program>` para verificar os endpoints HTTP.
- Swashbuckle e `Microsoft.AspNetCore.Mvc.Testing` foram fixados em versões compatíveis com .NET 10.

**Correções realizadas**

- Identifiquei uma referência do Swagger incompatível no primeiro teste de compatibilidade e ajustei para a versão 10.0.1.
- Ajustei o projeto de testes unitários com `FrameworkReference` para acessar os tipos do ASP.NET Core.

**Revisões realizadas**

- `dotnet build RoboCoffee.slnx`: sucesso, zero avisos e zero erros.
- Testes unitários: 1 aprovado, 0 falhas.
- Testes de integração: 3 aprovados, 0 falhas.
- Health check manual: `200 OK`, `Healthy`.
- Swagger UI manual: `200 OK`.
- OpenAPI manual: `200 OK`.

## 015 - Modelagem do domínio

**Contexto**

Implementei a Etapa 3 do plano de commits com os modelos essenciais de catálogo e pedidos.

**Contribuição da IA**

A IA criou enumerações e entidades ricas para produto, pedido e item de pedido, além de uma exceção de domínio e métodos para snapshot, cálculo do total e fluxo de status.

**Decisões confirmadas**

- `Product` valida nome, preço e estoque negativo.
- `OrderItem` preserva nome e preço do produto no momento da criação.
- `Order` calcula o total a partir dos itens e exige pelo menos um item antes da confirmação.
- `OrderStatus` segue `Received`, `Preparing`, `Ready`, `Delivered` e `Cancelled`.
- Somente pedidos em `Received` podem ser cancelados.
- Itens não podem ser alterados após a criação.

**Revisões realizadas**

- `dotnet build RoboCoffee.slnx --no-restore --disable-build-servers -m:1`: sucesso, zero avisos e zero erros.
- Testes unitários: 11 aprovados, 0 falhas.
- Testes de integração: 3 aprovados, 0 falhas.

**Resultado**

O domínio principal está modelado e coberto por testes focados, aguardando revisão e commit da Etapa 3.

**Commit relacionado**

`839ed4b` - `feat: add catalog and order domain models`

## 016 - Configuração da persistência EF Core

**Contexto**

Implementei a Etapa 4 do plano de commits, preparando EF Core, PostgreSQL, DbContext, configurações por entidade e migration inicial.

**Contribuição da IA**

A IA criou o `RoboCoffeeDbContext`, os mapeamentos de `Product`, `Order` e `OrderItem`, o registro de dependência da infraestrutura e a migration `Initial`.

**Decisões confirmadas**

- Entity Framework Core usa Npgsql com PostgreSQL.
- Tabelas: `products`, `orders` e `order_items`.
- Enumerações são persistidas como texto.
- Decimais são persistidos com precisão `(18,2)`.
- Índices foram criados nos campos consultados com mais frequência.
- Check constraints protegem preço, estoque, total e quantidade no banco.
- `OrderItem` é configurado como filho de `Order` com deleção em cascata.
- Credenciais não foram incluídas nos arquivos de configuração.
- A migration pode ser criada e aplicada por um tool local `dotnet-ef`.

**Revisões realizadas**

- Restore da solution concluído.
- Build concluído com zero avisos e zero erros.
- Testes unitários: 12 aprovados.
- Testes de integração: 3 aprovados.
- Migration `20261120190438_Initial` reconhecida pelo EF Core.
- API iniciou em `http://localhost:5128` com o DbContext registrado.
- PostgreSQL local não estava disponível, portanto a migration não pôde ser aplicada nesta máquina.

**Commit relacionado**

`422d147` - `feat: configure database and initial migration`

## 017 - CRUD de produtos

**Contexto**

Implementei a Etapa 5 do plano de commits com contratos, caso de uso, persistência e endpoints REST do catálogo.

**Contribuição da IA**

A IA criou contratos para produto, interface e implementação do catálogo, controller REST e erros HTTP no exception handler.

**Decisões confirmadas**

- CRUD implementado em `POST`, `GET`, `PUT` e `DELETE`.
- `DELETE` desativa o produto em vez de removê-lo fisicamente.
- Listagem possui paginação obrigatória entre página maior que zero e tamanho de 1 a 100.
- Produtos desativados não aparecem em consultas.
- A implementação usa um service específico de catálogo, sem repository genérico.
- Integrações HTTP de teste usam EF Core InMemory.

**Correções realizadas**

- O teste inicial revelou que Npgsql e InMemory estavam registrados simultaneamente.
- Corrigi o service provider do teste removendo a configuração Npgsql antes de registrar InMemory.

**Revisões realizadas**

- Build: sucesso, zero avisos e zero erros.
- Testes unitários: 12 aprovados.
- Testes de integração: 8 aprovados.
- OpenAPI: `/api/products` e `/api/products/{id}` reconhecidos.
- API iniciou em `http://localhost:5128`.
- Operação real com PostgreSQL não pôde ser feita sem uma instância local.

## 018 - Recuperação da sessão e falha ao aplicar Keep Changes

**Contexto**

Na sessão anterior, `tests/RoboCoffee.IntegrationTests/ProductCatalogTests.cs` sofreu uma incompatibilidade entre alterações acumuladas no disco e o estado mostrado pelo painel de alterações do VS Code.

Foram feitas duas alterações no arquivo antes da tentativa de usar o botão Keep Changes. Ao confirmar o Keep, o VS Code mostrou o erro:

```text
Could not keep changes to ProductCatalogTests.cs
ProductCatalogTests.cs: The changes to 'ProductCatalogTests.cs' were updated after they were shown.
Try again.
'ProductCatalogTests.cs' changed before its changes could be kept.
```

**Diagnóstico da IA**

A IA classificou o problema como uma divergência entre o conteúdo usado pelo editor para montar a ação Keep/Undo e o arquivo alterado posteriormente no disco. Os arquivos não deviam ser reconstruídos a partir de entradas stale do painel de alterações.

**Ações realizadas**

- Preservei o estado completo no handoff de sessão, incluindo o código final de `ProductCatalogTests.cs`.
- Não restaurei nem aceitei alterações obsoletas do VS Code.
- Iniciei uma nova sessão de chat.
- Restaurar o arquivo no disco pela fonte completa registrada no handoff.
- Validação do build e dos testes após a recuperação.

**Decisão confirmada**

- Quando ocorre uma divergência entre alterações do editor e alterações no disco, encerrar o chat atual e abrir uma nova sessão é uma recuperação segura.
- O handoff deve registrar os arquivos problemáticos, o estado completo necessário para recuperação e o próximo passo do projeto.
- O arquivo no disco e os documentos locais são a fonte de verdade.
- Não repetir Keep/Undo stale quando o editor não conseguir reconciliar uma alteração.

**Resultado**

`ProductCatalogTests.cs` foi recuperado no disco e os testes continuam funcionando:

- build: sucesso, 0 warnings, 0 errors;
- testes unitários: 12 aprovados;
- testes de integração: 8 aprovados.

**Validação manual realizada**

Com PostgreSQL acessível na porta 5432, a API foi iniciada em `http://localhost:5128` e validada manualmente:

- `/health`: `200 OK` com `Healthy`;
- `/swagger`: `200 OK`;
- `/openapi/v1.json`: `200 OK`;
- `POST /api/products`: produto criado com `201`;
- `GET /api/products/{id}`: produto recuperado com `200`;
- `GET /api/products?page=1&pageSize=10`: listagem paginada retornada com `200`;
- `PUT /api/products/{id}`: dados atualizados com `200`;
- `DELETE /api/products/{id}`: `204 No Content`;
- consulta posterior ao produto desativado: `404 Not Found`.

**Commit relacionado**

`6b9f826` - `feat: add product catalog endpoints`

## 019 - FluentValidation e ProblemDetails dos produtos

**Contexto**

Após confirmar a Etapa 5 localmente, implementei a Etapa 6 para validar payloads de criação e atualização antes de executar a camada de infraestrutura.

**Contribuição da IA**

A IA adicionou validadores para nome, categoria, tamanho, preço e estoque, registrou os validadores no ASP.NET Core e traduziu `ValidationException` para `400 Bad Request` com `ValidationProblemDetails`.

**Decisões confirmadas**

- Nome é obrigatório e limitado a 200 caracteres.
- Categoria e tamanho devem corresponder aos enums de domínio.
- Preço deve ser maior que zero.
- Estoque, quando informado, não pode ser negativo.
- Erros são agrupados por nome de propriedade em `errors`.
- Regras sintáticas ficam na entrada HTTP; regras de entidade continuam no domínio.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 12 aprovados.
- Testes de integração: 10 aprovados.
- Validação manual: `POST /api/products` com payload inválido retornou `400` e `errors` para nome, categoria, tamanho, preço e estoque.

**Correções realizadas**

A IA cometeu um erro ao indicar que FluentValidation já estaria pronta no ensaio do Commit 005, quando sua implementação pertencia ao Commit 006.

Foi necessária uma revisão e uma conversa para realinhar as instruções de documentação do projeto, corrigindo o Commit 005 como pendente nessa funcionalidade e registrando corretamente sua implementação no Commit 006.

## 020 - Criação de pedidos e cálculo do total

**Contexto**

Implementei a Etapa 7 com o endpoint de criação de pedidos, contratos HTTP, caso de uso, persistência EF Core e testes focados de integração.

**Contribuição da IA**

A IA criou os contratos `CreateOrderRequest`, `CreateOrderItemRequest`, `OrderResponse` e `OrderItemResponse`, o service `IOrderService/OrderService`, o `OrdersController` e o validador do pedido.

**Decisões confirmadas**

- `POST /api/orders` cria pedidos no estado `Received`.
- Todo pedido exige pelo menos um item.
- Quantidade deve ser maior que zero.
- Produtos são buscados no mesmo contexto da criação.
- Nome e preço são copiados para os itens no momento da criação.
- O total é calculado pela soma dos subtotais no domínio.
- Pedido e itens são persistidos na mesma operação EF Core.
- PostgreSQL usa transação explícita; InMemory mantém o mesmo fluxo sem transação porque não oferece suporte no provider.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 12 aprovados.
- Testes de integração: 14 aprovados.
- Validação manual:
  - saúde da API: `200`;
  - criação do produto: `201`;
  - criação do pedido: `201`;
  - total calculado: `24.00`;
  - preço unitário snapshot: `12.00`;
  - subtotal: `24.00`.

## 021 - Snapshot histórico de preço e nome do produto

**Contexto**

A Etapa 8 fechou formalmente a garantia de snapshot iniciada pela criação do pedido na Etapa 7.

**Contribuição da IA**

A IA adicionou um teste de integração que cria um pedido, altera o nome e o preço do produto e consulta o pedido persistido diretamente no `RoboCoffeeDbContext`.

**Decisões confirmadas**

- `OrderItem` mantém `ProductNameSnapshot` e `UnitPriceSnapshot`.
- Alterar o catálogo não recalcula itens ou totais existentes.
- O teste prova persistência, não apenas resposta em memória.
- A implementação de domínio já copiava os dados na criação do pedido.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 12 aprovados.
- Testes de integração: 15 aprovados.
- Um teste específico confirmou que nome e preço originais permanecem persistidos após a atualização do produto.

## 022 - Regras de indisponibilidade e quantidade mínima

**Contexto**

Finalizei a Etapa 9 reforçando a cobertura dos casos obrigatórios de criação de pedidos: quantidade maior que zero e rejeição de produtos indisponíveis.

**Contribuição da IA**

A IA acrescentou um teste de integração que envia quantidade zero e confirmou por HTTP que o pedido é rejeitado com `400 Bad Request`.

**Decisões confirmadas**

- Quantidade menor que um é invalidada pela FluentValidation antes da infraestrutura.
- `OrderItem.Create` mantém a mesma validação no domínio.
- Produto indisponível é rejeitado pelo domínio com `DomainException`.
- O exception handler traduz o conflito de domínio para `409 Conflict`.
- Produto ativo e disponível é necessário para criar o item.

**Correções realizadas**

- Fechada a lacuna de teste da integração: quantidade zero agora possui um caso HTTP específico.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 12 aprovados.
- Testes de integração: 16 aprovados.
- Validação manual:
  - quantidade zero: `400`;
  - produto indisponível: `409`.

## 023 - Fluxo de situação e cancelamento de pedidos

**Contexto**

Implementei os endpoints e o caso de uso necessários para avançar e cancelar pedidos pela API.

**Contribuição da IA**

A IA adicionou `OrderNotFoundException`, métodos de avanço e cancelamento no service, endpoints REST e testes de integração para estados válidos e inválidos.

**Decisões confirmadas**

- Fluxo: `Received -> Preparing -> Ready -> Delivered`.
- Cancelamento permitido apenas em `Received`.
- Avançar pedido entregue ou cancelado gera `409 Conflict`.
- Cancelar pedido fora de `Received` gera `409 Conflict`.
- Pedido inexistente gera `404 Not Found`.
- Estados continuam sendo alterados pelos métodos de domínio.

**Revisões realizadas**

- Build: sucesso, 0 warnings, 0 errors.
- Testes unitários: 12 aprovados.
- Testes de integração: 20 aprovados.
- Validação manual:
  - `Received -> Preparing -> Ready -> Delivered`;
  - avanço após `Delivered`: `409`;
  - cancelamento em `Received`: `Cancelled`;
  - cancelamento fora de `Received`: `409`.
