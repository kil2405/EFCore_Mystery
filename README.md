# MysteryBox EF Core Conversion (Visual Studio 2022)

> A .NET 8 / ASP.NET Core Web API + EF Core (MySQL) port of the **mysterybox** repo structure (controllers, services, models, DB tables, and CSV resource loader).

## Open & Run
1. Open `MysteryBox.EFCore.sln` in **Visual Studio 2022** (v17.8+).
2. Update `src/MysteryBox.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`.
3. Create database & schema via EF Migrations:
   ```bash
   dotnet tool install -g dotnet-ef
   cd src/MysteryBox.Api
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. Press **F5** → Swagger at `/swagger`.

## What’s included
- EF Core models mapping to original tables:
  - `tb_user`, `tb_user_item`, `tb_user_connect_log` (composite PK), `tb_click_count_log`, `tb_gold_acq_log`,
    `tb_item_acq_log`, `tb_item_reward`, `tb_reward_gold`, `tb_global_ranking`.
- Controllers mirroring Java packages:
  - `InitController`, `CorsDebugController`, `ErrorController`,
  - `UserController` (login / nickname / terms), `ContentsController` (click/equip),
  - `RankingController` (global/top, submit, tiebreak endpoint).
- Services:
  - `JwtTokenService`, `RedisService` (StackExchange.Redis), `GeoIpService` (MaxMind), `ResourceLoader` (CSV).
- Folders to drop original **CSV** files under `src/MysteryBox.Api/ResourceDB/CSV/`.
- `DBScript/GameDB/Table/` created (you can copy SQL from your repo here if needed).

## Notes on Assets
- **GeoLite2-City.mmdb** is not bundled due to license/size. If you want IP → Country, place the file at the project root (same as the Java repo) or copy to the app content root.
- Resource CSVs are **not** embedded; this template provides the loader and folder. Copy your CSVs (e.g., `tb_country.csv`, `tb_item_list.csv`, ...) into `ResourceDB/CSV/` and call `POST /api/admin/resources/rebuild`.

## Mapping (Java → C#)
- `controller/*` → `Controllers/*`
- `repository/*` (JPA) → `DbContext` + LINQ (`AppDbContext`)
- `config/*` (Redis, Datasource) → `appsettings.json` + DI services
- `common/* (TimeCalculation, constants)` → `Common/*`
- `VO`, `request/response` → `DTOs/*`

> This conversion is a faithful structure & behavior port. If you want a 1:1 endpoint contract with the original Java responses, share the exact payload schemas and we’ll align DTOs precisely.
