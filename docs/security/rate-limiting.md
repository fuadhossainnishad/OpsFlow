# API Rate-Limiting Assessment

## Status

Reviewed; enforcement is deferred until the API hosting and ingress topology
are defined. No rate limiter is currently registered or enabled in the API.

## Findings

- Registration and login are anonymous endpoints and are the clearest candidates
  for abuse protection.
- The repository's Docker Compose files define supporting services, but do not
  host the API or define an ingress proxy/load balancer.
- The API does not configure forwarded headers or a trusted proxy/network list.
- An in-process limit partitioned by `RemoteIpAddress` would use the connection
  peer address. Behind an unconfigured proxy that may identify the proxy rather
  than the caller; across multiple API instances, each instance would maintain
  its own limit.

## Decision

Do not enable an IP-based application limit until deployment defines how client
addresses are established and whether API instances share limiter state. Choose
the enforcement point after that topology is known: an ingress/gateway policy,
an application limiter with an explicitly trusted client-address source, or a
shared limiter if the deployment requires consistent limits across instances.

When implemented, start with anonymous registration and login. Define limits,
partition keys, and operational responses from measured traffic and abuse
requirements. Return `429 Too Many Requests` using the documented Problem
Details contract and include `Retry-After` when the chosen policy can determine
it. Avoid account-only lockout behavior that would let callers deny service to
other users.

## Reassessment prerequisites

Before turning on limits, document the API's ingress path, trusted proxy
addresses, instance count and scaling behavior, and whether shared limiter
storage is available. Then test both direct and proxied requests to confirm
clients cannot choose their own partition identity.
