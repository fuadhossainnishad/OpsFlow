# OpsFlow — Agent Engineering Instructions

## 1. Purpose

OpsFlow is an enterprise operations/workforce management platform built as a production-oriented .NET application.

This repository is developed with both human engineers and AI coding agents.

AI agents are expected to behave as software engineers working within an existing system, not as autonomous code generators.

The repository, source code, tests, configuration, and ADRs are the primary sources of truth.

---

## 2. Engineering Principles

Follow these principles unless an existing ADR explicitly establishes a different decision:

- Prefer correctness over speed.
- Prefer simple, explicit designs over unnecessary abstraction.
- Preserve existing architectural boundaries.
- Keep business rules inside the appropriate domain/application layer.
- Keep infrastructure concerns out of the domain.
- Keep HTTP concerns out of application business logic.
- Favor dependency inversion and testability.
- Prefer small, cohesive changes.
- Avoid unrelated refactoring.
- Avoid speculative features.
- Do not introduce dependencies without justification.
- Do not duplicate existing abstractions.
- Do not weaken architectural constraints merely to make implementation easier.

When existing code establishes a consistent pattern, follow that pattern unless there is a documented reason to improve it.

---

## 3. Repository Source of Truth

Use the following priority when determining project behavior:

1. Executable source code
2. Automated tests
3. Database/configuration definitions
4. ADRs
5. Technical documentation
6. Roadmap and project-context documentation

If documentation conflicts with implementation, investigate the discrepancy rather than blindly following either source.

Never invent behavior that is not supported by the repository.

---

## 4. Agent Workflow

For every non-trivial task:

### Phase 1 — Understand

Before editing:

- Read the relevant project documentation.
- Locate the affected feature/module.
- Inspect existing implementation patterns.
- Inspect relevant interfaces and abstractions.
- Inspect related domain entities/value objects.
- Inspect existing tests.
- Check git status.
- Identify dependencies and architectural boundaries.

Do not scan the entire repository unless the task genuinely requires it.

### Phase 2 — Plan

Determine:

- What actually needs to change.
- Which files should change.
- Which existing patterns should be reused.
- What tests are required.
- What could be affected.

For non-trivial changes, provide a short implementation plan before modifying code.

### Phase 3 — Implement

Implement the smallest complete solution.

Rules:

- Do not modify unrelated files.
- Do not silently change public contracts.
- Do not remove existing tests.
- Do not weaken assertions.
- Do not bypass validation.
- Do not suppress errors merely to obtain a passing build.
- Do not introduce speculative abstractions.

### Phase 4 — Validate

Prefer validation in this order:

1. Targeted test(s)
2. Relevant project test suite
3. Broader solution tests when appropriate
4. Build

Use the narrowest validation that gives meaningful confidence, then expand when the change warrants it.

Never claim that tests passed unless they were actually executed.

Report:

- commands executed
- tests passed/failed
- build status
- relevant warnings
- remaining issues

---

## 5. Test Engineering

Tests are part of the product's engineering quality.

When adding or changing behavior:

- Test observable behavior.
- Test success paths.
- Test important failure paths.
- Test authorization and tenant boundaries where relevant.
- Test domain invariants.
- Test persistence behavior where appropriate.
- Preserve deterministic tests.
- Prefer focused unit tests for application/domain behavior.
- Use integration tests for actual infrastructure and cross-layer behavior.

Do not write tests merely to increase coverage percentage.

Coverage is a signal, not the objective.

Do not alter production behavior solely to satisfy a coverage tool unless the uncovered path represents meaningful behavior.

---

## 6. Security

Treat security as a system property.

Never:

- expose secrets
- print credentials
- commit `.env` files containing secrets
- weaken authentication
- bypass authorization
- bypass tenant isolation
- disable security validation to make tests pass
- trust client-provided organization/user identity without appropriate verification

When handling identity, authorization, tenancy, files, tokens, or external integrations, inspect the existing security model before changing behavior.

---

## 7. Multi-Tenancy

OpsFlow is designed around organization/tenant isolation.

Any feature accessing organization-owned data must preserve tenant boundaries.

Do not assume that possession of an entity ID is sufficient authorization.

When modifying queries, repositories, handlers, or endpoints involving organization-owned resources, verify:

- organization scope
- authenticated user scope
- authorization requirements
- resource ownership/membership rules

Tenant isolation must not be treated as optional filtering.

---

## 8. Database Changes

Do not modify the database schema casually.

Before changing persistence:

- inspect existing entity configuration
- inspect migrations
- inspect relationships
- inspect indexes
- inspect concurrency configuration
- inspect tenant constraints
- inspect existing repository patterns

Never run destructive database operations without explicit authorization.

Do not reset, drop, or recreate databases as a shortcut for debugging unless explicitly requested.

---

## 9. API Changes

Before changing an API contract:

- inspect existing request/response contracts
- inspect validation
- inspect authorization
- inspect error handling
- inspect OpenAPI behavior
- inspect existing endpoint conventions
- inspect relevant integration tests

Avoid breaking existing clients without explicit approval.

---

## 10. Concurrency and Reliability

OpsFlow contains enterprise-oriented reliability concerns such as optimistic concurrency and background/outbox processing.

Do not remove concurrency checks or reliability mechanisms merely to simplify implementation.

When changing workflows involving:

- tasks
- approvals
- time entries
- notifications
- files
- background processing
- outbox messages

consider failure, retries, duplicate execution, race conditions, and transaction boundaries.

---

## 11. Agent Context Efficiency

AI context is a finite engineering resource.

Agents should minimize unnecessary context consumption.

Prefer:

- targeted file discovery
- targeted searches
- reading only relevant files
- existing abstractions
- existing tests
- focused test execution

Avoid:

- dumping entire source trees
- repeatedly reading the same files
- reading generated artifacts unnecessarily
- inspecting unrelated modules
- repeating repository-wide searches
- rebuilding unrelated projects
- producing large explanations when a concise engineering report is sufficient

Before searching, formulate the smallest useful search target.

---

## 12. Documentation

Documentation must describe the system accurately.

Do not create documentation merely for volume.

When implementation changes architecture, security, public APIs, database design, or an important engineering decision, update the relevant documentation or ADR.

When an API feature or contract is complete, document its endpoint, authorization and tenant requirements, request/query parameters, response/error shape, and a manual test example under `docs/api/`. Link the reference from `docs/api/README.md` and `docs/README.md`.

Do not document speculative functionality as implemented.

Clearly distinguish:

- implemented
- in progress
- planned
- proposed

---

## 13. ADRs

Architectural decisions belong in `docs/adr/`.

Create an ADR when a decision has meaningful long-term consequences, such as:

- architecture
- persistence strategy
- tenancy model
- authentication strategy
- major infrastructure choice
- messaging/background processing
- significant security design
- major API compatibility decision

Do not create an ADR for routine implementation details.

---

## 14. Dependencies

Before adding a package:

1. Determine whether the repository already provides the capability.
2. Determine whether the .NET framework or existing dependency is sufficient.
3. Consider maintenance and security implications.
4. Explain why the dependency is necessary.

Avoid dependency proliferation.

---

## 15. Git Safety

Before substantial changes:

- inspect `git status`
- understand existing uncommitted work
- do not overwrite unrelated user changes

Never use destructive git commands as a shortcut.

Do not reset, clean, force checkout, or discard user changes without explicit permission.

---

## 16. Secrets and Sensitive Configuration

Never expose or reproduce:

- passwords
- API keys
- tokens
- private certificates
- connection secrets
- credential files

Use placeholders when documenting configuration.

Do not inspect secret material unless it is specifically required and authorized for the task.

---

## 17. Existing Warnings

Fluent Assertions may emit a licensing/community-license warning during tests.

This warning is informational and is not itself a test failure.

Do not treat the warning as a failing test unless the test process actually fails.

---

## 18. Known Environment

Primary development environment:

- Ubuntu 24.04 LTS
- .NET 10
- ASP.NET Core 10
- Entity Framework Core 10
- SQL Server
- Docker
- xUnit
- Fluent Assertions

The solution file is:

`OpsFlow.slnx`

---

### Local Development Services

- OpsFlow may require local TCP access to services running on `localhost` (e.g. SQL Server, RabbitMQ).
- If the sandbox blocks required local TCP access, request elevated permission rather than changing code to work around the restriction.
- Never bypass or weaken security controls just to satisfy sandbox limitations.

---

## 19. Final Reporting

After completing a task, report concisely:

### Changed

Files/features modified.

### Why

The engineering reason for the change.

### Validation

Exact tests/builds executed and their results.

### Remaining

Known issues, blockers, or follow-up work.

Do not claim production readiness without sufficient validation.

---

## 20. Default Agent Behavior

When uncertain:

- inspect before assuming
- ask when requirements are genuinely ambiguous
- preserve existing behavior
- make the smallest safe change
- verify with tests
- document significant decisions

The goal is not to maximize code output.

The goal is to maintain a correct, secure, understandable, testable, and evolvable software system.
