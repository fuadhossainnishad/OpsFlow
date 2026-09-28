# Next

## Completed
- Worker/RabbitMQ/Outbox reliability — committed and pushed.
- API/auth hardening — validated JWT configuration at startup, required authentication by default for API routes, explicitly kept registration/login anonymous, and added invalid bearer-token coverage. Focused integration tests passed (5/5).
- API authorization and tenant-isolation verification — audited controller authorization policies and existing organization-scoped integration coverage; added missing/malformed organization-header rejection coverage. Focused tenant-isolation integration tests passed (5/5).
- API response-contract consistency review — aligned team creation with the existing project/organization creation convention by returning 201 Created and a resource location. Team API integration tests passed (7/7).
- API validation convention review — added email-format validation to registration, login, and member-invitation request contracts; malformed addresses are rejected by API model validation. Focused API integration tests passed (14/14).
- Time-entry collection pagination — added bounded `page`/`pageSize` parameters and `hasNextPage` metadata while retaining project, task, and UTC interval filters. Documented response and manual requests in `docs/api/time-entries.md`; focused integration tests passed (9/9).
- Time-entry collection sorting — added allowlisted `sortBy` (`startedAtUtc`, `createdAtUtc`) and `sortOrder` (`asc`, `desc`) parameters with deterministic ID tie-breaking. Updated manual API guidance; time-entry integration tests passed (9/9), and focused time-entry unit tests passed (6/6).
- Consistent API error responses — standardized empty HTTP error responses and centralized exception responses as RFC 7807 Problem Details, including request path and trace ID; unexpected errors do not expose exception details. Documented the contract in `docs/api/errors.md`.
- OpenAPI response metadata review — documented shared validation/authentication/authorization/server-error responses on protected controllers, declared missing model-validation/auth failures for auth endpoints, and described generated document availability and metadata conventions in `docs/api/openapi.md`.
- API rate-limiting assessment — confirmed there is no API deployment/ingress topology or trusted-proxy configuration in the repository; documented why rate limits are deferred until client-address trust and instance sharing are defined, with registration/login as initial candidates.
- OpenAPI bearer authentication and operation errors — described JWT bearer security on non-anonymous operations and added Problem Details response metadata for handler-backed 404/409 failures; updated the OpenAPI reference.
- API versioning and deprecation review — documented the existing `/api/v1` route convention, compatibility expectations, and the migration information required before a version is retired. Deferred adding versioning infrastructure until a second API version is approved.

## Current
- No active milestone.

## Next
- Add focused API contract verification for OpenAPI security requirements and Problem Details response metadata, continuing Phase 3 API hardening.

## Rule
After every milestone, update this file.
