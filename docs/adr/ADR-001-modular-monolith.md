# ADR-001: Use a Modular Monolith Architecture

## Status

Accepted

## Context

OpsFlow is an enterprise workforce and operations management platform containing multiple business capabilities including identity, organizations, projects, tasks, workforce management, approvals, notifications, reporting, auditing, integrations, and billing.

The system needs strong domain boundaries and the ability to evolve as usage increases.

Introducing independently deployed microservices at the beginning would add significant operational complexity before there is sufficient evidence that independent deployment or scaling is required.

## Decision

OpsFlow will initially use a modular monolith architecture.

The application will be divided into explicit business modules while remaining deployed as a cohesive application.

The initial architectural layers are:

- Domain
- Application
- Infrastructure
- Contracts
- API
- Worker

Business modules will maintain explicit boundaries and should avoid unnecessary coupling.

## Consequences

### Positive

- Lower operational complexity.
- Easier local development.
- Strong domain boundaries.
- Easier transactional consistency.
- Simpler deployment.
- Clear future extraction points if a module genuinely requires independent scaling.

### Negative

- Requires discipline to maintain module boundaries.
- A poorly designed module can still create coupling inside the monolith.
- Independent service deployment is not available initially.

## Future Evolution

A module may become an independent service only when there is a demonstrated requirement such as:

- Independent scaling.
- Independent deployment cadence.
- Strong isolation requirements.
- Distinct operational ownership.
- Technology/runtime requirements that differ from the main application.

Architecture will evolve based on measurable requirements rather than premature distribution.
