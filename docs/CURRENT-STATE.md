# OpsFlow — Current State

> Last maintained: 2026-09-27

## 1. Development Status

OpsFlow is in active backend development.

The core enterprise foundation and a substantial set of application workflows are implemented.

Current work is focused on strengthening behavioral test coverage, validating cross-layer behavior, resolving infrastructure issues, and progressively hardening the system toward production quality.

---

## 2. Solution

Solution:

`OpsFlow.slnx`

Primary projects include:

- `OpsFlow.Contracts`
- `OpsFlow.Domain`
- `OpsFlow.Application`
- `OpsFlow.Infrastructure`
- `OpsFlow.Api`

Test projects include unit and integration test suites.

---

## 3. Runtime / Development Environment

Primary development environment:

- Ubuntu 24.04.5 LTS
- .NET 10
- ASP.NET Core 10
- Entity Framework Core 10
- SQL Server
- Docker

---

## 4. Implemented Domain Areas

Current implementation includes functionality related to:

- Identity
- Users
- Organizations
- Memberships
- Roles
- Organization invitations
- Teams
- Team membership
- Team leadership
- Tasks
- Time entries
- Approvals
- Notifications
- Files
- Audit logs
- Refresh tokens
- User credentials
- Outbox messages
- Local file storage

The exact implementation status of individual workflows should always be verified against source code and tests.

---

## 5. Application Workflows Tested

Behavioral unit coverage has been expanded across important application workflows, including:

- team-member operations
- task operations
- time-entry operations
- team operations
- user credential behavior
- file storage
- approval workflows
- current-user behavior
- membership behavior
- audit behavior
- notification behavior
- organization behavior
- invitation workflows
- user behavior
- refresh-token behavior
- file-record behavior
- role behavior
- task creation
- file upload
- notification read handling
- team-member removal
- time-entry updates
- outbox behavior
- approval cancellation
- approval approval
- team-lead changes
- invitation acceptance

This list represents testing work completed during the current development cycle; it should not be interpreted as a guarantee that every possible behavior in each area is fully covered.

---

## 6. Current Unit Test Baseline

As of 2026-09-27:

**259 / 259 unit tests passing**

- Failed: 0
- Skipped: 0

The Fluent Assertions licensing/community-license message printed during test execution is informational and does not represent a test failure.

---

## 7. Coverage Strategy

Coverage work is being driven by meaningful behavioral gaps rather than by percentage alone.

High-value areas have been prioritized based on uncovered executable behavior and architectural importance.

The objective is to test:

- success paths
- validation failures
- authorization failures
- tenant isolation
- missing resources
- concurrency conflicts
- persistence failures
- external-service failures
- cleanup/rollback behavior
- important domain invariants

Generated code, compiler-generated state machines, OpenAPI-generated code, and similar artifacts should not drive artificial coverage work.

---

## 8. Known Infrastructure Issue

There has been an SQL Server authentication issue while running EF database updates against the local SQL Server container.

The command builds successfully but has encountered SQL Server login/authentication failure for the configured `sa` user.

Container:

`opsflow-sqlserver`

This should be investigated separately from application unit-test correctness.

Do not treat this infrastructure issue as evidence that application code or unit tests are failing.

---

## 9. Important Engineering Constraints

- Preserve multi-tenant boundaries.
- Preserve authorization behavior.
- Preserve optimistic concurrency.
- Preserve auditability.
- Preserve outbox/reliability mechanisms.
- Avoid destructive database operations.
- Avoid exposing secrets.
- Do not weaken tests to make the suite pass.
- Do not introduce speculative architecture.

---

## 10. Immediate Development Direction

The immediate engineering direction is:

1. Continue meaningful application-handler behavioral coverage.
2. Identify remaining high-risk uncovered behavior.
3. Validate integration behavior.
4. Resolve local SQL Server/EF database-update configuration.
5. Review API authorization and tenant isolation.
6. Strengthen reliability and failure handling.
7. Progress toward production-readiness validation.

---

## 11. State Classification

Use these labels when updating this document:

### Implemented
Verified in source code and/or tests.

### In Progress
Currently being actively developed.

### Blocked
Requires resolution before meaningful progress can continue.

### Planned
Approved future work.

### Proposed
Possible future direction that has not been committed.
