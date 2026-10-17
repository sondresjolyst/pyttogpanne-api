# Changelog

## [1.1.0](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.5...v1.1.0) (2026-10-17)


### Features

* **auth:** end sessions at the API on logout and after a password reset ([#37](https://github.com/sondresjolyst/pyttogpanne-api/issues/37)) ([c7cc82c](https://github.com/sondresjolyst/pyttogpanne-api/commit/c7cc82ceec6f28651b28abc27fee0386809db82c))
* **auth:** send reset and invite emails through SjolystInnovation.AppKit.Auth ([#47](https://github.com/sondresjolyst/pyttogpanne-api/issues/47)) ([5d4139b](https://github.com/sondresjolyst/pyttogpanne-api/commit/5d4139bcdcdb54f88764c79db233de90fd758a4c))
* **auth:** serve sign-in and password reset from SjolystInnovation.AppKit.Auth ([#43](https://github.com/sondresjolyst/pyttogpanne-api/issues/43)) ([e6ff9b7](https://github.com/sondresjolyst/pyttogpanne-api/commit/e6ff9b7a3d36e34cca5e7325ee138eb82e650bcd))
* **auth:** use SjolystInnovation.AppKit.Auth for Identity, bearer tokens and token cleanup ([#41](https://github.com/sondresjolyst/pyttogpanne-api/issues/41)) ([826c2f3](https://github.com/sondresjolyst/pyttogpanne-api/commit/826c2f3d910182de6ee37711e3928f0019280265))
* **branding:** add the branding endpoint the app uses ([#33](https://github.com/sondresjolyst/pyttogpanne-api/issues/33)) ([86fb4a0](https://github.com/sondresjolyst/pyttogpanne-api/commit/86fb4a0eb5f286829218f6bda1fcdaac8011fd85))
* **email:** send through SjolystInnovation.AppKit.Email instead of the Brevo SDK ([#42](https://github.com/sondresjolyst/pyttogpanne-api/issues/42)) ([c59f930](https://github.com/sondresjolyst/pyttogpanne-api/commit/c59f93090a9cf43136bf35f695b448e707f1dd70))
* **legal:** serve legal pages from SjolystInnovation.AppKit.SiteSettings ([#51](https://github.com/sondresjolyst/pyttogpanne-api/issues/51)) ([c337fee](https://github.com/sondresjolyst/pyttogpanne-api/commit/c337feec23944f5033be3babe23bbad870bcb938))
* **settings:** move settings, company info and branding onto SjolystInnovation.AppKit.SiteSettings ([#49](https://github.com/sondresjolyst/pyttogpanne-api/issues/49)) ([2b86f3e](https://github.com/sondresjolyst/pyttogpanne-api/commit/2b86f3e806252ebe501fd14fa2664ed6e03fe950))
* use SjolystInnovation.AppKit.Hosting for startup and rate limiting ([#40](https://github.com/sondresjolyst/pyttogpanne-api/issues/40)) ([3deaaec](https://github.com/sondresjolyst/pyttogpanne-api/commit/3deaaec9090a276607def368e48187119109b4c9))
* **users:** delete accounts through SjolystInnovation.AppKit.Auth ([#45](https://github.com/sondresjolyst/pyttogpanne-api/issues/45)) ([4985d9d](https://github.com/sondresjolyst/pyttogpanne-api/commit/4985d9db861be8a564709355eaeff75a8250aa1d))
* **users:** serve user admin, invites and first-admin seeding from SjolystInnovation.AppKit.Auth ([#44](https://github.com/sondresjolyst/pyttogpanne-api/issues/44)) ([9ea0ef0](https://github.com/sondresjolyst/pyttogpanne-api/commit/9ea0ef0efa8d61cc96da0e8369d09eb01a43ba3f))


### Bug Fixes

* rate limit by the client IP from trusted proxies only ([#38](https://github.com/sondresjolyst/pyttogpanne-api/issues/38)) ([03ceb28](https://github.com/sondresjolyst/pyttogpanne-api/commit/03ceb288b2acfa1ac9da1d1c70f17ad55be5a22f))
* trust the cluster pod network as a proxy ([#39](https://github.com/sondresjolyst/pyttogpanne-api/issues/39)) ([0400465](https://github.com/sondresjolyst/pyttogpanne-api/commit/0400465b142c9c0e9c53148751fb4d6083746ae8))


### Dependencies

* bump SjolystInnovation.AppKit.Auth to 0.8.0 ([#48](https://github.com/sondresjolyst/pyttogpanne-api/issues/48)) ([6df9871](https://github.com/sondresjolyst/pyttogpanne-api/commit/6df9871f749f4ee4b0f902080695476481a0c2fe))
* **nuget:** Bump the skiasharp group with 2 updates ([#35](https://github.com/sondresjolyst/pyttogpanne-api/issues/35)) ([8fb22a4](https://github.com/sondresjolyst/pyttogpanne-api/commit/8fb22a4b4ce4300e150f4e92a5c3c23ebd2c687e))

## [1.0.5](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.4...v1.0.5) (2026-10-04)


### Dependencies

* **nuget:** bump `Moq` from 4.20.72 to 4.21.0 ([#31](https://github.com/sondresjolyst/pyttogpanne-api/issues/31)) ([d607431](https://github.com/sondresjolyst/pyttogpanne-api/commit/d6074314e984067156b22c5be137d017e7b3e163))
* **nuget:** Bump Mapster and Mapster.DependencyInjection ([#29](https://github.com/sondresjolyst/pyttogpanne-api/issues/29)) ([0babfa8](https://github.com/sondresjolyst/pyttogpanne-api/commit/0babfa8e3e99009e6fe1a1554e766e9a2cb95294))

## [1.0.4](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.3...v1.0.4) (2026-10-03)


### Bug Fixes

* evict every refresh token past the cap, not just one ([#26](https://github.com/sondresjolyst/pyttogpanne-api/issues/26)) ([72a29c0](https://github.com/sondresjolyst/pyttogpanne-api/commit/72a29c08b329a98df70627738dfcf3fc4d1c5ed3))

## [1.0.3](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.2...v1.0.3) (2026-09-27)


### Dependencies

* **nuget:** bump `System.IdentityModel.Tokens.Jwt` from 8.22.0 to 8.23.0 ([#24](https://github.com/sondresjolyst/pyttogpanne-api/issues/24)) ([190e1b4](https://github.com/sondresjolyst/pyttogpanne-api/commit/190e1b4b58286a77371f21647bf52f40d6f2c4ff))
* **nuget:** Bump the skiasharp group with 2 updates ([#23](https://github.com/sondresjolyst/pyttogpanne-api/issues/23)) ([b19bd59](https://github.com/sondresjolyst/pyttogpanne-api/commit/b19bd5939249fae0bc0f24c85e9cea8e5f5f9366))

## [1.0.2](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.1...v1.0.2) (2026-09-25)


### Bug Fixes

* add an anonymous health endpoint ([#20](https://github.com/sondresjolyst/pyttogpanne-api/issues/20)) ([9dbe002](https://github.com/sondresjolyst/pyttogpanne-api/commit/9dbe002d3728e06c5f8654d099432b7e1404244b))

## [1.0.1](https://github.com/sondresjolyst/pyttogpanne-api/compare/v1.0.0...v1.0.1) (2026-09-21)


### Dependencies

* **nuget:** bump `Microsoft.EntityFrameworkCore.InMemory` from 10.0.11 to 10.0.12 ([#12](https://github.com/sondresjolyst/pyttogpanne-api/issues/12)) ([64fb2dc](https://github.com/sondresjolyst/pyttogpanne-api/commit/64fb2dc7e298b87fbcd42d6968b10f7fc05bbd3c))
* **nuget:** bump `xunit.v3` from 4.0.0 to 4.0.1 ([#15](https://github.com/sondresjolyst/pyttogpanne-api/issues/15)) ([6258d5f](https://github.com/sondresjolyst/pyttogpanne-api/commit/6258d5fd69e10d9a0a1c9caa8689534cf406c5db))
* **nuget:** Bump the aspnetcore group with 6 updates ([#18](https://github.com/sondresjolyst/pyttogpanne-api/issues/18)) ([a6fac95](https://github.com/sondresjolyst/pyttogpanne-api/commit/a6fac955e27006017bfca2e1bcc4fef775fe8f96))
* **nuget:** Bump the skiasharp group with 2 updates ([#19](https://github.com/sondresjolyst/pyttogpanne-api/issues/19)) ([a06c824](https://github.com/sondresjolyst/pyttogpanne-api/commit/a06c824dbea30a5cdbd850053d5bfa349309f2b2))

## 1.0.0 (2026-09-19)


### Features

* add photo galleries and advertising marking ([76b46ad](https://github.com/sondresjolyst/pyttogpanne-api/commit/76b46ad3c9a5ec1e456b0098161dc23ef859f583))
* add recipe API for Pyttogpanne ([b184920](https://github.com/sondresjolyst/pyttogpanne-api/commit/b1849206ecb3dfce296de4fcc17b887f1a927cc9))
* rewrite the legal pages ([249d3ce](https://github.com/sondresjolyst/pyttogpanne-api/commit/249d3ced0ff96ea31642aede4178b2a7bf5564c1))
* seed sample recipes in development ([44d6142](https://github.com/sondresjolyst/pyttogpanne-api/commit/44d61424a9a69f37a50aa7cccbd40f6e7b9e6933))


### Bug Fixes

* allow the dev site as a cors origin ([efc115a](https://github.com/sondresjolyst/pyttogpanne-api/commit/efc115ab2d188345b20c0202e04d2285bd7c205e))
* trust only configured proxies for X-Forwarded-For ([f2d6e96](https://github.com/sondresjolyst/pyttogpanne-api/commit/f2d6e96f489dfe0b57483d2e4a8d410aff7b7f92))
* use a user secrets id of our own ([671137e](https://github.com/sondresjolyst/pyttogpanne-api/commit/671137e60da2fda1a5f906eb046e11fbe3534338))
