<p align="center">
  <img src="docs/pyttogpanne.png" alt="Pyttogpanne" width="220">
</p>

# pyttogpanne-api

API for Pyttogpanne. Content is written in
[pyttogpanne-app](https://github.com/sondresjolyst/pyttogpanne-app) and read by
[pyttogpanne-mobile](https://github.com/sondresjolyst/pyttogpanne-mobile).

## Stack

ASP.NET Core 10, PostgreSQL through EF Core and Npgsql, ASP.NET Identity with
JWT, Mapster, Serilog, AspNetCoreRateLimit, Brevo.

## Quick start

```bash
dotnet restore
dotnet ef database update   # needs a local Postgres, see appsettings.Development.json
dotnet run                  # Swagger at /swagger
```

## Environment

Production reads these from the cluster secret.

| Variable | Used for |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Jwt__Key`, `Jwt__Issuer` | JWT signing key and issuer. The key must match the app's `PYTTOGPANNE_API_JWT_SECRET` |
| `BrevoSettings__ApiKey`, `BrevoSettings__SenderEmail`, `BrevoSettings__SenderName` | Transactional email |
| `Seed__AdminEmail`, `Seed__AdminPassword` | First admin, created at startup |
| `Site__BaseUrl` | Used in links sent by email |
| `Storage__ImagesPath` | Mount for uploaded images, `/data/images` in the cluster |

## What it serves

| Area | Holds |
| --- | --- |
| Recipes | Ingredients and ordered steps, times, servings, difficulty, categories, photos. Drafts stay unpublished until released to the app |
| Categories | The filters recipes are listed under |
| Articles | Gear and trail tips, written in markdown |
| Legal pages | Terms, privacy and cookies, per language |
| Accounts | Sign-in, JWT and refresh tokens, roles. Invitation only, no public sign-up |

Browse `/swagger` on a running instance for the current surface.

## Health

| Path | Reports |
| --- | --- |
| `/health` | The process is up. No dependency checks, so a database outage does not restart the pod |
| `/health/ready` | The database connection. Fails while Postgres is unreachable, which takes the pod out of its Service |

Both are anonymous, and both are blocked at the ingress: only the kubelet
reaches them, over the pod address.

## Languages

`Constants/Locales.cs` lists the supported locales and `no` is the default.
Legal text is stored per language. To add one, add its tag to
`Locales.Supported` and seed its text.

## Deployment

Image [`sondresjo/pyttogpanne-api`](https://hub.docker.com/r/sondresjo/pyttogpanne-api)
on Docker Hub, chart `pyttogpanne-api` in
[tumogroup-charts](https://github.com/sondresjolyst/tumogroup-charts), applied by
Flux from [tumo-flux](https://github.com/sondresjolyst/tumo-flux) to
`pyttogpanne-dev` and `pyttogpanne-prod`.

The container runs as the non-root `app` user with a read-only root filesystem,
so anything written at runtime needs a volume. Uploaded images go to the
`/data/images` mount, and the data protection key ring to `/home/app/.aspnet`.

Migrations run at startup, so a deploy against a cold database can take a while.
The startup probe allows for that before the liveness probe can restart the pod.

A push to `main` builds the `dev` tag. A release-please release builds `vX.Y.Z`,
tags it `latest` and opens a chart bump against
[tumogroup-charts](https://github.com/sondresjolyst/tumogroup-charts). Cluster
secrets are created by
[`scripts/pyttogpanne/bootstrap.sh`](https://github.com/sondresjolyst/tumo-platform/blob/main/scripts/pyttogpanne/bootstrap.sh)
in [tumo-platform](https://github.com/sondresjolyst/tumo-platform).

## License

Proprietary. Copyright (c) 2026 Sondre Sjølyst.
