# THIARA

THIARA is an open-source **TARA tool** (Threat Analysis and Risk Assessment) for automotive and embedded security engineering, implementing the ISO 21434 workflow.

## Setup

### Docker (recommended)

```bash
docker pull <your-registry>/thiara:latest

mkdir -p /srv/thiara/data

docker run -d \
  --name thiara \
  --restart unless-stopped \
  -p 8080:8080 \
  -v /srv/thiara/data:/app/Data \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="DataSource=/app/Data/app.db;Cache=Shared" \
  <your-registry>/thiara:latest
```

Or with `docker-compose.yml` (set `thiara_IMAGE` in your environment):

```bash
docker compose up -d
```

On first start with an empty database, a one-time registration URL is printed to stdout. Open it to create the initial admin account. All subsequent users are invited through the in-app flow.

### Local development

Prerequisites: [.NET 10 SDK](https://dotnet.microsoft.com/download)

```bash
git clone <repo-url>
cd tara-tool

dotnet restore src/thiara.csproj
dotnet run --project src/thiara.csproj
```

The app starts on `https://localhost:5001` (or the port shown in the terminal). Migrations are applied automatically on startup.

## Overview

The domain model is a strict hierarchy:

```
Project
  └── Item Definition     — the system/component under analysis
        └── Asset         — data or functional asset (tagged)
              └── Damage Scenario   — CIA impact ratings (Safety / Financial / Operational / Privacy)
                    └── Threat Scenario   — STRIDE category, risk value
                          └── Attack Path   — feasibility factors (time, expertise, equipment …)
                                └── Attack Step
```

Projects are multi-user. Access is controlled per-project with four permission levels: Read, Write, Manage, and Owner.
