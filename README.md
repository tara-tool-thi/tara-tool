# THIARA

THIARA is an open-source **TARA tool** (Threat Analysis and Risk Assessment) for automotive and embedded security engineering, implementing the ISO 21434 workflow.

## Setup

### Docker (recommended)

A minimal compose file is as follows:

```bash
services:
  tara:
    image: ghcr.io/tara-tool-thi/thiara:latest
    restart: always
    volumes:
      - ./tara:/app/Data
      - ./imprint.html:/app/wwwroot/imprint.html
    ports:
      - 8080:8080
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_HTTP_PORTS=8080
      - ConnectionStrings__DefaultConnection=DataSource=/app/Data/app.db;Cache=Shared
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

The app starts locally (see link in terminal). Migrations are applied automatically on startup.

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
