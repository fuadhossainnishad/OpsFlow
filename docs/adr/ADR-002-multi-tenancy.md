# ADR-002: Tenant-Isolated Multi-Tenancy

## Status

Accepted

## Context

OpsFlow is a SaaS platform where multiple organizations use the same application infrastructure.

Tenant data must remain isolated and a user must only be able to access resources belonging to organizations for which they have appropriate membership and permissions.

## Decision

OpsFlow will use a shared application and shared relational database with logical tenant isolation.

Tenant-owned resources will be associated with an Organization identifier.

The authenticated user's organization membership will determine the tenant context.

Client-provided organization identifiers will not be trusted as the source of authorization.

Authorization will be enforced server-side.

## Consequences

### Positive

- Efficient infrastructure utilization.
- Simplified deployment.
- Strong SaaS model.
- Easier cross-tenant operational management.

### Risks

Tenant isolation becomes a critical security boundary.

Automated tests must verify that users cannot access resources belonging to other organizations.

Database queries must be designed to prevent accidental cross-tenant access.

## Future Evolution

If regulatory, scale, or isolation requirements change, selected tenants may eventually be moved to isolated databases or infrastructure without changing the core domain model.
