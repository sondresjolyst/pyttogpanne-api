# pyttogpanne-api

Serves recipes, categories and gear tips to the Pyttogpanne app.

## Architecture

Vertical slice: each feature under `Features/` owns its endpoints, contracts and validators, and
registers its own routes through `IEndpoint`. No controllers, no MediatR. Validation is
FluentValidation via `WithValidation<T>()`; errors come back as ProblemDetails.

```
Features/        one folder per slice
Models/          EF entities and the DbContext
Services/        image storage, email, background jobs
Infrastructure/  endpoint registration, validation filter, seeding
```

Recipes are the core entity: ingredients and steps are ordered child rows, categories a link
table, images a reference to `ContentImages`. Recipes are drafts until published.

## Endpoints

| Route | Access |
| --- | --- |
| `GET /api/recipes` | anonymous; `category` and `search` filters, `all=true` adds drafts for an admin |
| `GET /api/recipes/sync` | anonymous; every published recipe in full, `since` returns only changes |
| `GET /api/recipes/{slug}` | anonymous; drafts admin-only |
| `POST/PUT/DELETE /api/recipes` | admin |
| `/api/recipe-categories` | read anonymous, write admin |
| `/api/gear` | read anonymous, write admin |
| `/api/content-images` | upload admin, read anonymous |

`sync` also returns `deletedSlugs`. Deleting, unpublishing or renaming a recipe writes a
tombstone row, so a client that cached it knows to drop it; re-publishing clears the tombstone.

## Running

```sh
dotnet run
```

Swagger at `/swagger`. Connection string, `Jwt:Key`, `Cors:AllowedOrigins` and `TrustedProxies`
come from configuration — user secrets locally, environment variables in the cluster. Pending
migrations are applied at startup.

`TrustedProxies` lists the CIDRs allowed to set `X-Forwarded-For`. Leave it empty when the API is
not behind a proxy; a wildcard would let any caller forge the client IP the rate limiter reads.

Create the database with `ENCODING UTF8`.

## Tests

```sh
dotnet build pyttogpanne-api.Tests/pyttogpanne-api.Tests.csproj
./pyttogpanne-api.Tests/bin/Debug/net10.0/pyttogpanne-api.Tests.exe
```

xUnit v3 builds the test project as an executable and is invoked directly; `dotnet test`
discovers nothing on some local setups. Handlers are called as plain static methods against an
in-memory context, so there is no test host to configure.

## Migrations

```sh
dotnet ef migrations add <Name> --project pyttogpanne-api.csproj
```
