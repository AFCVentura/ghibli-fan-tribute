# Ghibli Fan Tribute

[![CI/CD](https://github.com/AFCVentura/ghibli-fan-tribute/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/AFCVentura/ghibli-fan-tribute/actions/workflows/ci-cd.yml)
[![CodeQL](https://github.com/AFCVentura/ghibli-fan-tribute/actions/workflows/codeql.yml/badge.svg)](https://github.com/AFCVentura/ghibli-fan-tribute/actions/workflows/codeql.yml)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?logo=blazor&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-4-06B6D4?logo=tailwindcss&logoColor=white)
![Azure Container Apps](https://img.shields.io/badge/Azure-Container_Apps-0078D4?logo=microsoftazure&logoColor=white)
[![Live site](https://img.shields.io/badge/live-ghiblitribute.jventura.dev-2ea44f)](https://ghiblitribute.jventura.dev)

Project made with Blazor to honor Studio Ghibli.

This project used TailwindCSS for styling.

Live at **https://ghiblitribute.jventura.dev**

Here are some screenshots of finished UIs:

![](https://github.com/AFCVentura/ghibli-fan-tribute/blob/main/navhero.png)
![](https://github.com/AFCVentura/ghibli-fan-tribute/blob/main/films.png)
![](https://github.com/AFCVentura/ghibli-fan-tribute/blob/main/aboutfooter.png)

## Tests

`tests/GhibliTribute.Tests` has 50 tests written with xUnit v3 and bUnit:

- the services that call external APIs (Studio Ghibli API, OMDb, Resend, Cloudflare Turnstile) run against a fake HTTP handler, so the tests never touch the internet;
- the film catalog and the analytics settings;
- Blazor components (form errors, footer in English and Portuguese, theme and language switches in the nav menu).

```bash
dotnet test --solution GhibliTribute.sln
```

## CI/CD

Every push to `main` goes through [`ci-cd.yml`](.github/workflows/ci-cd.yml):

1. **Test**: restores, builds and runs the whole test suite. If any test fails, nothing is deployed.
2. **Build the image**: builds the Docker image (multi-stage, runtime-only final image) and pushes it to GitHub Container Registry, tagged with the commit hash.
3. **Deploy**: signs in to Azure with OIDC (no stored passwords) and points the Azure Container App to the new image.
4. **Health check**: waits until the new revision is provisioned, running and healthy, then hits the real domain. If the new version does not come up, the run fails instead of reporting a false success.

Pull requests only run the tests.

Also in place:

- **CodeQL** ([`codeql.yml`](.github/workflows/codeql.yml)) scans the C# code and the workflows for security issues on every push, on pull requests and weekly.
- **Dependabot** ([`dependabot.yml`](.github/dependabot.yml)) checks NuGet, npm, GitHub Actions and Docker base images every week and opens pull requests, which go through the same tests.
