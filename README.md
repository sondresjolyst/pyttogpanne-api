<p align="center">
  <img src="docs/pyttogpanne.png" alt="Pyttogpanne" width="220">
</p>

<p align="center">
  The backend behind Pyttogpanne — recipes, categories, gear tips, and the images that go with them.
</p>

---

pyttogpanne-api is the API for **Pyttogpanne** — turmat cooked in one pan on a gas
burner. It stores the recipes Pyttogpanne writes, serves them to the mobile app,
and backs the admin console in
[pyttogpanne-app](https://github.com/sondresjolyst/pyttogpanne-app).

## What it does

- **Recipes** — ingredients and steps in order, servings, times, difficulty and
  categories. Drafts stay hidden until they are published.
- **Offline sync** — the app pulls every published recipe in one call and asks
  for changes since its last sync. Recipes that are deleted, unpublished or
  renamed are reported back so a cached copy is dropped.
- **Gear and trail tips** — markdown articles alongside the recipes.
- **Images** — uploaded once, stored on a mounted volume and served in the size
  the caller asks for.
- **Accounts** — login, JWT + refresh tokens, password reset and roles. Admins
  are invited; there is no public registration.
- **Site content** — legal pages, company details and settings for the admin
  console.

Built as vertical slices (minimal API endpoints + FluentValidation), with
PostgreSQL via EF Core.

---

## For developers

<details>
<summary>Run, configure, and test from source</summary>

### Stack

ASP.NET Core · PostgreSQL (EF Core / Npgsql) · ASP.NET Identity + JWT · Mapster ·
Serilog · AspNetCoreRateLimit · SkiaSharp · Brevo.

### Run locally

```bash
dotnet restore
dotnet run     # Swagger at /swagger
```

Pending migrations are applied at startup, against `ConnectionStrings:DefaultConnection`.
Create the database with `ENCODING UTF8`: recipe text is full of æ, ø and å.

The first admin is seeded from configuration; further admins are invited from the
admin console.

### Configuration

| Setting | What it's for |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string. |
| `Jwt:Key`, `Jwt:Issuer` | Signs and validates tokens. The app verifies with the same key. |
| `Cors:AllowedOrigins` | Origins allowed to call the API with credentials. |
| `TrustedProxies` | CIDRs allowed to set `X-Forwarded-For`. Empty when there is no proxy in front; a wildcard lets any caller forge the client IP the rate limiter reads. |
| `Storage:ImagesPath` | Where uploaded images are written. |
| `Site:BaseUrl` | Used in emails and generated links. |

### Tests

```bash
dotnet build pyttogpanne-api.Tests/pyttogpanne-api.Tests.csproj
./pyttogpanne-api.Tests/bin/Debug/net10.0/pyttogpanne-api.Tests.exe
```

xUnit v3 builds the test project as an executable and is invoked directly;
`dotnet test` discovers nothing on some local setups. Handlers are called as
plain static methods against an in-memory context, so there is no test host to
configure.

### Migrations

```bash
dotnet ef migrations add <Name> --project pyttogpanne-api.csproj
```

### Layout

```
Features/        one folder per slice, each mapping its own routes
Models/          EF entities and the DbContext
Services/        image storage, email, background jobs
Infrastructure/  endpoint registration, validation filter, seeding
Migrations/      EF migrations
```

</details>
