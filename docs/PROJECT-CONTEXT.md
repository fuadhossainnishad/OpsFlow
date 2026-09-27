# OpsFlow — Project Context

## 1. Overview

OpsFlow is an enterprise operations and workforce management platform.

The project is being developed as a production-oriented modular monolith using the modern .NET ecosystem.

Its purpose is to provide a structured platform for organizations to manage operational work, teams, tasks, time, approvals, notifications, files, auditing, and related workflows.

The project also serves as a serious engineering reference implementation demonstrating maintainable enterprise application architecture.

---

## 2. Product Direction

OpsFlow is intended to evolve into a multi-tenant operational platform where an organization can manage its people, teams, operational work, approvals, time records, communication events, and supporting documents from a unified system.

The platform is designed with enterprise concerns from the beginning rather than adding them after the core functionality is complete.

Important cross-cutting concerns include:

- multi-tenancy
- authentication
- authorization
- auditability
- optimistic concurrency
- transactional consistency
- background processing
- reliable event/outbox processing
- validation
- observability
- testability
- security

---

## 3. Major Capability Areas

The current system includes or is actively implementing capabilities around:

### Identity

User identity and authenticated-user context.

### Organizations

Organization/tenant boundaries and organization-level ownership.

### Memberships

Relationships between users and organizations.

### Invitations

Organization invitation workflows and invitation acceptance.

### Roles and Authorization

Role-based access control and permission-oriented authorization.

### Teams

Team creation, membership, leadership, and team-management workflows.

### Tasks

Task creation and task lifecycle management.

### Time Tracking

Time-entry management and concurrency-aware updates.

### Approvals

Approval requests and approval/rejection/cancellation workflows.

### Notifications

User notifications and notification read state.

### Files

File metadata and storage abstraction.

### Audit

Audit logging for important operational actions.

### Outbox

Reliable persistence and background delivery of application events/messages.

---

## 4. Architectural Direction

OpsFlow follows a modular-monolith architecture.

The solution separates concerns across projects including:

- Contracts
- Domain
- Application
- Infrastructure
- API
- Unit Tests
- Integration Tests

The architecture intentionally keeps the domain and application logic independent from infrastructure details where practical.

The application layer coordinates use cases.

The domain layer owns business rules and invariants.

Infrastructure implements persistence and external-system abstractions.

The API layer exposes application functionality over HTTP.

Detailed architectural decisions are documented under:

`docs/adr/`

and:

`docs/architecture/`

---

## 5. Multi-Tenancy

Organizations represent the primary tenant boundary.

Organization-owned resources must be accessed within the appropriate organization context.

The system uses tenant context abstractions so application workflows do not need to directly depend on HTTP-specific mechanisms for determining the active organization.

Tenant isolation is a security and correctness requirement.

---

## 6. Engineering Philosophy

OpsFlow is intentionally being developed with:

- explicit business rules
- strong boundaries
- testable application handlers
- repository abstractions
- unit-of-work coordination
- domain entities
- authorization checks
- tenant-aware data access
- concurrency protection
- auditable workflows
- reliable background processing

The project should favor understandable enterprise engineering over framework-driven complexity.

---

## 7. Current Development Approach

Development is incremental.

Each feature should normally move through:

1. domain behavior
2. application use case
3. infrastructure implementation
4. API integration
5. unit tests
6. integration tests
7. security/authorization review
8. documentation where necessary

The exact sequence may vary depending on the feature.

---

## 8. Source of Truth

The implementation is authoritative.

Project documentation describes intended architecture and development direction but must not be treated as evidence that functionality exists.

Feature status should be verified against source code and tests.

---

## 9. Future Direction

Potential future areas include:

- richer operational dashboards
- advanced reporting
- scheduling
- workflow automation
- deeper real-time capabilities
- integrations with external services
- stronger observability
- production deployment automation
- AI-assisted operational workflows

These are directional possibilities unless explicitly marked as committed roadmap work.
