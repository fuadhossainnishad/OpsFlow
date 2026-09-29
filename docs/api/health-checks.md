# Health checks

The API exposes anonymous operational health endpoints. These routes are not part of the versioned business API.

## Liveness

`GET /health/live` returns `200 OK` while the API process can serve requests. It does not check external dependencies, so an unavailable database does not make liveness fail.

## Readiness

`GET /health/ready` checks whether SQL Server accepts a connection using the API's configured `OpsFlowDatabase` connection string.

- `200 OK` means the database is reachable.
- `503 Service Unavailable` means readiness failed.

RabbitMQ is not part of API readiness because message consumption and delivery are owned by the worker. The API's outbox writes are persisted in SQL Server.

## Manual checks

```sh
curl -i http://localhost:5000/health/live
curl -i http://localhost:5000/health/ready
```

Use the application's actual local URL and port when running the API.
