# OpsFlow Documentation

This directory contains the engineering documentation for OpsFlow.

## Documentation hierarchy

### Project-level

- `PROJECT-CONTEXT.md` — what OpsFlow is, product direction, major capabilities.
- `CURRENT-STATE.md` — verified implementation state and current engineering status.
- `ROADMAP.md` — planned and proposed future work.

### Architecture

`architecture/`

Contains system architecture, layer boundaries, modules, data flow, and other structural documentation.

### API

`api/`

Contains HTTP/API conventions, contracts, error handling, authorization, and API-specific guidance.

- [API reference](api/README.md)
- [API error response contract](api/errors.md)
- [OpenAPI guidance](api/openapi.md)
- [API versioning policy](api/versioning.md)

### Database

`database/`

Contains persistence architecture, schema strategy, tenancy, concurrency, migrations, and database-specific decisions.

### Security

`security/`

Contains authentication, authorization, tenant isolation, secrets, threat considerations, and security engineering guidance.

- [API rate-limiting assessment](security/rate-limiting.md)

### ADRs

`adr/`

Architecture Decision Records documenting decisions with meaningful long-term consequences.

---

## Documentation rules

Documentation must describe the actual system accurately.

The source code and tests are authoritative.

Use these classifications when describing implementation:

- **Implemented** — verified in source code/tests.
- **In Progress** — currently being developed.
- **Blocked** — work cannot reasonably continue until a dependency is resolved.
- **Planned** — intentionally scheduled future work.
- **Proposed** — possible future direction, not committed.

Do not document speculative functionality as implemented.

Do not duplicate detailed technical information across multiple documents.

Prefer linking to the authoritative document instead.

When an architectural decision changes, update the relevant ADR.

When implementation materially changes the documented architecture, update the corresponding technical documentation.

When the project's current development state changes, update `CURRENT-STATE.md`.

When roadmap priorities change, update `ROADMAP.md`.
