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
