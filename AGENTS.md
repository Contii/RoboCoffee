# AGENTS.md

## Project documentation

- Human overview: `README.md`
- Technical architecture: `docs/ARCHITECTURE.md`
- Commit backlog: `docs/COMMIT_PLAN.md`
- AI record: `docs/AI_JOURNAL.md`
- Private interview study: `.context/INTERVIEW_ESSAYS.md`

## Mandatory workflow

- Implement mandatory requirements before "Vá Além".
- Never commit code that does not compile or fails existing tests.
- Run build and relevant tests before every commit.
- Never execute Git commands without showing the command and its effect first.
- Do not commit credentials, temporary files or files in `.context/`.

## Development workflow

At the start of a work round:

1. Read `AGENTS.md`, `README.md`, `docs/ARCHITECTURE.md`, `docs/COMMIT_PLAN.md` and, when present, `.context/SESSION_HANDOFF.md`.
2. Consult `docs/COMMIT_PLAN.md`, identify the next unfinished step and determine whether it needs to be added, split or removed before implementation.
3. Inspect `docs/ARCHITECTURE.md` for the technical constraints and decisions affected by that step.
4. Reconcile the backlog and architecture with the step, proposing documentation changes before or while implementing if a durable decision changed.

During implementation:

1. Implement the smallest complete and testable part of the step.
2. Add or update focused automated tests for behavior introduced or corrected.
3. Build and run the relevant tests.
4. Execute the application or integration scenario when it proves the behavior and document the exact manual check.
5. Review the working diff and explain the changed files.

Before proposing a commit:

1. Update documentation that reflects the current project state:
   - `README.md`: progress, commands and behavior that are now real.
   - `docs/ARCHITECTURE.md`: only when architecture or a relevant technical decision changes.
   - `docs/COMMIT_PLAN.md`: completed commits and new steps when needed.
   - `docs/AI_JOURNAL.md`: summarize the implementation session and decisions.
   - `.context/INTERVIEW_ESSAYS.md`: explain the implementation, interview talking points and manual start/test procedure.
2. Confirm build and test results.
3. Present the concise change summary, exact files expected in the commit, semantic commit message and Git commands with effects.
4. Wait for explicit approval before staging or committing.

Git operations remain local until the final repository push is explicitly requested.

## Commit review

Before staging or committing, present:

- A concise summary of the change.
- Files expected in the commit.
- Build and test results.
- One or more proposed semantic commit messages.
- The Git commands, including the effect of staging, unstaging or restoring files.

Do not execute `git add`, `git restore --cached`, `git commit`, `git push` or other Git mutating commands without explicit approval.

## Documentation upkeep

- Keep `README.md` synchronized with commands and functionality that actually work.
- Update `docs/ARCHITECTURE.md` only for durable architectural changes.
- Update `docs/COMMIT_PLAN.md` after completed and approved commits.
- Add an entry to `docs/AI_JOURNAL.md` after relevant implementation, debugging or architectural sessions.
- Update `.context/INTERVIEW_ESSAYS.md` after each implemented step with the interview explanation and the exact manual verification procedure.
- Summarize journal entries from available conversation context and repository evidence.
- Do not ask the user to copy AI outputs, inputs or tool results into the journal.
- For sensitive or uncertain session details, keep the summary short and state that the content was inferred or not verified.

## Git permissions

Only after explicit approval:

- `git add`
- `git restore --cached`
- `git rm --cached`
- `git commit`
- `git push`

## Architecture

- Use a pragmatic modular monolith.
- Keep controllers thin.
- Keep business rules in the domain.
- Separate API, Application, Domain and Infrastructure.
- Do not introduce repository abstractions, CQRS or Event Sourcing unless justified.
- Add abstractions only when they solve a current need.

## Stack

- .NET 10 / ASP.NET Core.
- EF Core and PostgreSQL.
- FluentValidation and ProblemDetails.
- xUnit, FluentAssertions and WebApplicationFactory.
- Docker Compose.
- Polly and Serilog.

## Commands

```powershell
dotnet restore
dotnet build
dotnet test
docker compose up --build
```

## Before changes

- Read `README.md`, `docs/ARCHITECTURE.md` and `docs/COMMIT_PLAN.md`.
- Make the smallest complete change.
- Add or update focused tests.
- Preserve existing behavior.
- Document directly related changes.
