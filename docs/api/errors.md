# API Error Responses

The API returns errors using the `application/problem+json` media type and the
RFC 7807 Problem Details shape. Responses include `type`, `title`, `status`,
`detail` when safe and available, and `instance` (the request path). They also
include a `traceId` extension that can be supplied to operators when reporting
an error.

Application exceptions are translated centrally: malformed requests and
argument errors return 400, authentication failures return 401, permission
failures return 403, missing resources return 404, and conflicts return 409.
Unexpected exceptions return 500 with a generic title and no exception detail.
Empty error responses, including unmatched routes and authentication or
authorization failures, are filled with the same Problem Details envelope.

ASP.NET Core model validation errors retain the standard
`ValidationProblemDetails` `errors` dictionary, along with `status`,
`title`, `type`, `instance`, and `traceId`.

Example (resource not found):

```json
{
  "type": "about:blank",
  "title": "Not Found",
  "status": 404,
  "instance": "/api/v1/example/00000000-0000-0000-0000-000000000001",
  "traceId": "0H..."
}
```

Validation example:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [ "The Email field is not a valid e-mail address." ]
  },
  "instance": "/api/v1/auth/register",
  "traceId": "0H..."
}
```

Clients should use the HTTP status and Problem Details fields rather than
parsing exception messages. The `traceId` is for support correlation and is
not an authorization token.
