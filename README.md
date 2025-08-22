<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net" alt=".NET 8" />
  <img src="https://img.shields.io/badge/EF%20Core-8-6DB33F" alt="EF Core 8" />
  <img src="https://img.shields.io/badge/Auth-JWT%20(HS256)-orange" alt="JWT" />
  <img src="https://img.shields.io/badge/DB-MySQL-4479A1?logo=mysql" alt="MySQL" />
  <img src="https://img.shields.io/badge/Cache-Redis-DC382D?logo=redis" alt="Redis" />
</p>

<h1 align="center">MysteryBox API (EF Core)</h1>

<p align="center">
  EF Core + MySQL 기반 게임/유저 API. JWT(HS256) 인증과 Redis를 활용한 <b>단일 세션(JTI)</b> 검증 및 <b>Refresh Token</b> 갱신을 지원합니다.
</p>

---

## ✨ 주요 특징

- **JWT 인증(HS256)**  
  - `iss`/`aud` 검증, **UTC** 기반 만료(`exp`), `ClockSkew=0`
  - 토큰 발급 시 `jti` 포함
- **단일 세션(싱글 로그인) 강제**  
  - 로그인 시 `auth:current:{userId}`에 최신 `jti` 저장  
  - 요청 시 헤더 토큰의 `jti`와 Redis 값을 비교하여 불일치일 경우 차단
- **Refresh Token 갱신(권장 플로우)**  
  - 로그인 시 **Access Token + Refresh Token** 동시 발급  
  - `POST /api/user/refresh` 로 Refresh Token 제출 → **새 Access/Refresh** 회전 발급  
  - Redis에 최신 Refresh Token 저장 및 역인덱스 관리(`auth:refresh:*` 키)
- **헤더 유연성**  
  - 표준: `Authorization: Bearer <token>`  
  - 비표준 허용: `Authorization: <token>` 또는 `X-Auth-Token: <token>`
- **EF Core + MySQL**  
  - 깔끔한 DbContext와 엔티티 설계, 마이그레이션 지원
- **Swagger UI**  
  - Bearer & X-Auth-Token 보안 스키마 제공

---

## 🗂 프로젝트 구조

```
src/
└─ MysteryBox.Api/
   ├─ Program.cs                      # 앱 엔트리: DI, Swagger, JWT, Redis/JTI 검증, 헤더 파싱
   ├─ Data/
   │  └─ AppDbContext.cs              # EF Core DbContext (MySQL)
   ├─ Controllers/
   │  ├─ Users/
   │  │  └─ UserController.cs         # 로그인, 닉네임, 리프레시(POST /api/user/refresh)
   │  └─ Debug/
   │     └─ DebugController.cs        # (옵션) /api/diag/* 진단 엔드포인트
   ├─ Services/
   │  ├─ JwtTokenService.cs           # JWT 발급(UTC, jti 포함) + Refresh Token 생성
   │  ├─ RedisService.cs              # Redis 헬퍼(String/SortedSet)
   │  ├─ GeoIpService.cs              # (선택) IP 기반 국가 등
   │  ├─ ResourceLoader.cs            # (선택) 리소스 로딩
   │  └─ JwtKeyProvider.cs            # 서명키 유틸(32바이트 이상 보정)
   ├─ Common/
   │  ├─ ICurrentUser.cs              # 현재 요청의 사용자 정보 인터페이스
   │  └─ CurrentUser.cs               # JWT 클레임(sub, unique_name) → UserId/Nickname
   ├─ DTOs/
   │  ├─ Requests/
   │  │  ├─ ReqSetNickname.cs         # { "Nickname": "..." } (UserId는 받지 않음)
   │  │  └─ ReqRefresh.cs             # { "RefreshToken": "..." }
   │  └─ Responses/
   │     └─ ResLoginDto.cs            # 로그인 응답: Token + RefreshToken 포함
   └─ Models/
      └─ Game/…                       # 도메인 엔티티(Users, Items, Logs 등)
```

---

## ⚙️ 설정 (appsettings.*)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mysterybox;User=root;Password=yourpassword;TreatTinyAsBoolean=false;"
  },
  "Jwt": {
    "Key": "change-me-32bytes-or-more",
    "Issuer": "MysteryBox",
    "Audience": "MysteryBoxAudience",
    "ExpiresInMinutes": 120,
    "RefreshDays": 14
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Auth": {
    "EnforceSingleSession": true
  }
}
```

> 환경변수 예시  
> `JWT__KEY`, `JWT__ISSUER`, `JWT__AUDIENCE`, `JWT__REFRESHDAYS`, `REDIS__CONNECTIONSTRING` 처럼 `__`로 중첩 키를 설정할 수 있습니다.

---

## 🧪 실행 방법 (개발)

```bash
# MySQL
docker run -d --name mysql -p 3306:3306   -e MYSQL_ROOT_PASSWORD=yourpassword mysql:8.0

# Redis
docker run -d --name redis -p 6379:6379 redis:7

# (마이그레이션을 쓰는 경우)
# dotnet ef database update -p src/MysteryBox.Api/MysteryBox.Api.csproj

# 실행
dotnet run --project src/MysteryBox.Api/MysteryBox.Api.csproj
```

> **Swagger**: 실행 후 `/swagger` 접속 → 상단 **Authorize**에서 토큰 입력

---

## 🔐 인증 & 갱신 흐름

```mermaid
sequenceDiagram
  participant C as Client
  participant API as MysteryBox.Api
  participant R as Redis

  rect rgb(240,240,255)
  Note over C,API: 로그인
  C->>API: POST /api/user/login (DeviceId)
  API-->>C: 200 { token (with jti), refreshToken }
  API->>R: SET auth:current:{sub} = jti (TTL=access.exp)
  API->>R: SET auth:refresh:current:{sub} = refresh (TTL=days)
  API->>R: SET auth:refresh:bytoken:{refresh} = sub (TTL=days)
  end

  rect rgb(240,255,240)
  Note over C,API: 일반 요청
  C->>API: POST /api/user/nickname (Authorization: Bearer <token>)
  API->>R: GET auth:current:{sub}  (== token.jti ?)
  R-->>API: jti
  API-->>C: 200 OK (일치 시)
  end

  rect rgb(255,245,240)
  Note over C,API: 토큰 만료 시 갱신
  C->>API: POST /api/user/refresh { refreshToken }
  API->>R: GET auth:refresh:bytoken:{refreshToken} -> sub
  API->>R: GET auth:refresh:current:{sub}  (== refreshToken ?)
  API-->>C: 200 { new token, new refreshToken }
  API->>R: SET auth:current:{sub} = newJti (TTL=access.exp)
  API->>R: DEL auth:refresh:bytoken:{oldRefresh}
  API->>R: SET auth:refresh:current:{sub} = newRefresh (TTL=days)
  API->>R: SET auth:refresh:bytoken:{newRefresh} = sub (TTL=days)
  end
```

---

## 🚀 빠른 사용법 (cURL)

```bash
# 1) 로그인: Access + Refresh 발급
curl -s -X POST http://localhost:5000/api/user/login   -H "Content-Type: application/json"   -d '{"DeviceId":"dev-1"}'
# -> { "userId": 1, "token": "...", "refreshToken": "...", ... }

# 2) 일반 요청 (토큰만으로 사용자 식별)
curl -s -X POST http://localhost:5000/api/user/nickname   -H "Authorization: Bearer <TOKEN>"   -H "Content-Type: application/json"   -d '{"Nickname":"Neo"}'

# 3) 갱신 (Access 만료 시)
curl -s -X POST http://localhost:5000/api/user/refresh   -H "Content-Type: application/json"   -d '{"RefreshToken":"<REFRESH_TOKEN_FROM_LOGIN>"}'
# -> { "token": "...", "refreshToken": "..." }
```

---

## 📚 API 요약

| Method | Path                  | Auth | Body                           | 설명 |
|------:|-----------------------|:----:|--------------------------------|------|
| POST  | `/api/user/login`     | ❌    | `{ "DeviceId": "dev-1" }`      | Access + Refresh 발급, Redis에 최신 `jti`/Refresh 저장 |
| POST  | `/api/user/nickname`  | ✅    | `{ "Nickname": "Neo" }`        | 헤더 토큰의 `sub`로 사용자 식별 |
| POST  | `/api/user/refresh`   | ❌    | `{ "RefreshToken": "..." }`    | Refresh 검증 후 Access/Refresh 회전 발급 |
| GET   | `/api/diag/why`       | (opt) | –                              | 토큰 파싱/JTI 비교 결과 |
| GET   | `/api/diag/claims`    | (opt) | –                              | 실제 인증 결과/실패 사유 |

---
