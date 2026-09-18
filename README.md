# pyttogpanne-api

Backend for Pyttogpanne — a Norwegian cookbook of one-pan trail meals. Serves the mobile app
(`pyttogpanne-mobile`) and the admin site (`pyttogpanne-app`).

Vertical slice architecture: minimal API endpoints, static handlers, FluentValidation,
ProblemDetails errors. Each slice registers itself through `IEndpoint`.

## Running

```sh
dotnet run
```

Swagger is at `/swagger`. Configure the connection string, JWT key and CORS origins through
`appsettings.Development.json` or user secrets.

The database must be created with `ENCODING UTF8` — recipe text is full of æ, ø and å.

## Endpoints

| Route | Access | What |
| --- | --- | --- |
| `GET /api/recipes` | anonymous | Published recipes; `category`, `search` filters. `all=true` adds drafts for an admin. |
| `GET /api/recipes/sync` | anonymous | Every published recipe in full, plus slugs to drop. `since` returns only what changed. |
| `GET /api/recipes/{slug}` | anonymous | One recipe, drafts admin-only. |
| `POST/PUT/DELETE /api/recipes` | admin | Editor writes. |
| `GET /api/recipe-categories` | anonymous | Categories, admin writes on the same route. |
| `GET /api/gear` | anonymous | Gear and trail tips, admin writes on the same route. |
| `POST /api/content-images` | admin | Image upload; recipes reference images by id. |

The app syncs with `/api/recipes/sync`, stores the response, and passes the previous `serverTime`
back as `since` next time. Unpublishing or deleting a recipe, and renaming one, all land in
`deletedSlugs` so a cached copy is dropped.

## Tests

```sh
dotnet build pyttogpanne-api.Tests/pyttogpanne-api.Tests.csproj
./pyttogpanne-api.Tests/bin/Debug/net10.0/pyttogpanne-api.Tests.exe
```

xUnit v3 builds the test project as an executable. `dotnet test` discovers zero tests on some
local setups; CI runs `dotnet test --no-build`.

## Migrations

```sh
dotnet ef migrations add <Name> --project pyttogpanne-api.csproj
```

Pending migrations are applied at startup.
