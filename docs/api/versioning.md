# API Versioning and Deprecation

## Current state

The implemented HTTP API uses the `/api/v1` route prefix. It is URL-versioned
by major contract version; there is no API-versioning package or separate
version negotiation mechanism. The route prefix is part of the public contract.

## Compatibility policy

- Keep backward-compatible additions and fixes within the current major
  version.
- Treat changes that remove or rename fields, change their meaning, or
  otherwise require existing clients to change as breaking.
- Introduce breaking contracts under a new route prefix, such as `/api/v2`.
  Keep the prior version available while clients migrate; do not silently
  redirect an old version to a contract with different behavior.
- Keep version-specific HTTP contracts in the API layer. Shared domain and
  application behavior can remain shared when the business rules are the same.

## Deprecation

Do not deprecate a version until a replacement and migration guidance are
available. A deprecation plan must name affected routes, the replacement,
required client changes, a support contact, and a retirement date. Publish the
plan in the API documentation and communicate it to affected consumers before
removing the old routes. Set the support window and any deprecation headers
when a concrete deployment and consumer lifecycle are established; neither is
currently configured.

## When to add versioning infrastructure

The route prefix is sufficient while only v1 is implemented. Reassess a
versioning package or shared version-routing infrastructure when a second
version is approved, so the implementation serves an actual compatibility
need rather than creating unused machinery.
