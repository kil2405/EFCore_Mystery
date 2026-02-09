# MysteryBox API (EF Core)

## 프로젝트 개요
- **Git**: https://github.com/kil2405/EFCore_Mystery.git
- **Framework**: ASP.NET Core (.NET 8.0)
- **ORM**: EF Core 8 (Pomelo.EntityFrameworkCore.MySql)
- **DB**: MySQL 8.0
- **Cache**: Redis (StackExchange.Redis)
- **Auth**: JWT (HS256) + Refresh Token (단일 세션 강제)
- **상태**: Active

## 프로젝트 구조
```
EFCore_Mystery/
├── src/MysteryBox.Api/
│   ├── Program.cs                    # 앱 엔트리 (DI, JWT, Redis JTI 검증)
│   ├── Controllers/
│   │   ├── Users/UserController.cs   # 로그인, 닉네임, 토큰 갱신
│   │   ├── Contents/ContentsController.cs  # 클릭, 장착
│   │   ├── Rank/RankingController.cs       # 랭킹 조회
│   │   └── Admin/InitController.cs         # 초기 데이터 로드
│   ├── Services/
│   │   ├── JwtTokenService.cs        # JWT 발급(UTC, jti) + Refresh Token
│   │   ├── RedisService.cs           # Redis 헬퍼 (String/SortedSet)
│   │   ├── ResourceLoader.cs         # CSV 리소스 로딩
│   │   └── JwtKeyProvider.cs         # 서명키 유틸 (32바이트 이상 보정)
│   ├── Common/
│   │   ├── ICurrentUser.cs / CurrentUser.cs  # JWT 클레임 -> UserId/Nickname
│   │   └── ConstantVal.cs, TimeCalculation.cs
│   ├── Data/AppDbContext.cs          # EF Core DbContext (MySQL)
│   ├── DTOs/                         # Request.cs, Response.cs
│   ├── Models/Game/                  # User, UserItem, ClickCountLog, GlobalRanking
│   ├── Migrations/
│   └── appsettings.*.json
├── Dockerfile
├── k8s/                              # Kubernetes 배포
└── MysteryBox.EFCore.sln
```

## 주요 API
| Method | Path | Auth | 설명 |
|--------|------|------|------|
| POST | /api/user/login | X | Access + Refresh Token 발급 |
| POST | /api/user/nickname | O | 닉네임 설정 (JWT sub 기반) |
| POST | /api/user/refresh | X | Refresh -> 새 토큰 회전 발급 |
| POST | /api/contents/click | O | 클릭 액션 |
| POST | /api/contents/equip | O | 아이템 장착 |
| GET | /api/ranking | O | 랭킹 조회 |

## 인증 흐름 (JWT + Redis 단일 세션)
1. 로그인 -> JWT(jti) + Refresh Token 발급
2. Redis `auth:current:{userId}` = jti 저장
3. 요청마다 헤더 jti vs Redis 비교 (단일 세션 강제)
4. 만료 시 Refresh Token으로 회전 갱신

## 코딩 규칙
- Minimal Hosting Model (Program.cs에 DI 설정 집중)
- Controller -> Service 계층 분리
- DTO: Request.cs, Response.cs로 통합
- PropertyNamingPolicy = null (PascalCase JSON)
- Singleton: RedisService, JwtTokenService, ResourceLoader
- Scoped: ICurrentUser/CurrentUser
- 환경별 설정: appsettings.{env}.json

## 빌드 및 실행
```bash
# 의존 서비스
docker run -d --name mysql -p 3306:3306 -e MYSQL_ROOT_PASSWORD=yourpassword mysql:8.0
docker run -d --name redis -p 6379:6379 redis:7

# 실행
dotnet run --project src/MysteryBox.Api/MysteryBox.Api.csproj
# Swagger: http://localhost:5000/swagger
```

## 에이전트 규칙 (필수)
1. **작업 전 `git pull` 실행**
2. **변경 후 README.md History + agent TASKS.md 업데이트**
3. **커밋 전 History 업데이트 필수**
4. **appsettings.*.json의 비밀번호/키 변경 금지**
5. **Migrations 수정 금지, 새로 생성만 허용**
