# Time Entries API

## List time entries

`GET /api/v1/time-entries` lists the authenticated user's time entries in the selected organization. The request requires the `TimeEntriesRead` permission and an `X-Organization-Id` header for an organization where the user has an active membership.

Supported query parameters:

| Parameter | Meaning | Default |
| --- | --- | --- |
| `projectId` | Keep entries for this project. | — |
| `taskId` | Keep entries for this task. | — |
| `fromUtc` | Include entries whose `StartedAtUtc` is at or after this value. | — |
| `toUtc` | Include entries whose `StartedAtUtc` is before this value. | — |
| `page` | One-based page number. Values below 1 are treated as 1. | `1` |
| `pageSize` | Requested number of entries per page, clamped to 1–100. | `20` |
| `sortBy` | `startedAtUtc` or `createdAtUtc`. | `startedAtUtc` |
| `sortOrder` | `asc` or `desc`. | `desc` |

Filters are applied together. The selected timestamp and entry ID are ordered in the requested direction so paging remains stable. Invalid sort values are rejected with `400 Bad Request`. The returned object contains the `Items`, normalized `Page` and `PageSize`, and `HasNextPage`.

Example response:

```json
{
  "items": [
    {
      "timeEntryId": "00000000-0000-0000-0000-000000000001",
      "organizationId": "00000000-0000-0000-0000-000000000002",
      "userId": "00000000-0000-0000-0000-000000000003",
      "projectId": "00000000-0000-0000-0000-000000000004",
      "taskId": null,
      "description": "Review deployment",
      "startedAtUtc": "2026-09-27T09:00:00Z",
      "endedAtUtc": "2026-09-27T09:30:00Z",
      "durationSeconds": 1800,
      "isManual": true,
      "createdAtUtc": "2026-09-27T09:30:00Z",
      "updatedAtUtc": null
    }
  ],
  "page": 1,
  "pageSize": 20,
  "hasNextPage": false
}
```

## Test manually

Set `API_URL`, `ACCESS_TOKEN`, and `ORGANIZATION_ID` to the local API URL and credentials for a user with `TimeEntriesRead` access to that organization. A valid access token and organization membership are required; the organization ID is not trusted without a server-side membership check.

```bash
curl -G "$API_URL/api/v1/time-entries" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "X-Organization-Id: $ORGANIZATION_ID" \
  --data-urlencode "page=1" \
  --data-urlencode "pageSize=10" \
  --data-urlencode "sortBy=startedAtUtc" \
  --data-urlencode "sortOrder=desc"
```

Filter by project and UTC interval:

```bash
curl -G "$API_URL/api/v1/time-entries" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "X-Organization-Id: $ORGANIZATION_ID" \
  --data-urlencode "projectId=<project-id>" \
  --data-urlencode "fromUtc=2026-09-01T00:00:00Z" \
  --data-urlencode "toUtc=2026-10-01T00:00:00Z" \
  --data-urlencode "page=1" \
  --data-urlencode "pageSize=25"
```

Follow `HasNextPage` and increment `page` to retrieve later pages. A successful response is `200 OK`; missing/invalid organization context is rejected, and unauthenticated or unauthorized requests are denied.
