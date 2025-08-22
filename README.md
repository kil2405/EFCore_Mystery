EF Core 스터디 시작

EF 마이그레이션을 통해 데이터 베이스 스키마 생성

dotnet tool install -g dotnet-ef
cd src/MysteryBox.Api
dotnet ef migrations add InitialCreate
dotnet ef database update

구조
Controller - api 요청과 응답을 담당
Repository - DbContext + LINQ
Config - Redis, DataSource