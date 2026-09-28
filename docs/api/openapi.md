# OpenAPI

The API uses ASP.NET Core's built-in OpenAPI generator. In Development, the
document is available at `GET /openapi/v1.json`. OpenAPI is not currently
mapped in other environments and the Development document endpoint allows
anonymous access. Keep it disabled in deployed environments unless publishing
the API schema there is an explicit decision. The document describes the controller routes;
success response schemas are inferred from action return types and explicit
`ProducesResponseType` metadata.

The document defines an HTTP bearer scheme using JWT access tokens. Operations
require that scheme unless their endpoint allows anonymous access. Registration
and login are anonymous; protected operations show the bearer requirement.

Protected controllers declare their shared failure responses:

| Status | Meaning |
| --- | --- |
| `400` | Request binding or validation failed. The body is `ValidationProblemDetails`. |
| `401` | Authentication is missing or invalid. |
| `403` | The user is authenticated but does not have the required permission. |
| `500` | An unexpected server error occurred. The body does not expose exception details. |

Authentication and organization creation endpoints declare their request and
operation-specific responses directly. Resource handlers that can return
missing-resource or conflict errors declare 404/409 response metadata on the
affected operations. Add `ProducesResponseType` metadata to an action when it
introduces a distinct expected response such as a conflict, missing resource,
or size limit. Those responses use the shared Problem Details contract
described in [Error responses](errors.md).

## Inspect the document locally

Run the API with `ASPNETCORE_ENVIRONMENT=Development`, then open
`/openapi/v1.json` on the local API host. The endpoint is only enabled in the
Development environment.
