# ================== Build stage ==================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# (옵션) 이미지 메타데이터
LABEL org.opencontainers.image.source="https://github.com/kil2405/EFCore_Mystery"

# 레포 전체를 복사하되, .dockerignore로 빌드에 불필요한 것들은 제외합니다.
# 이렇게 하면 ProjectReference가 있어도 복사 누락으로 인한 restore 실패가 없습니다.
COPY . .

# NuGet 패키지 캐시(빌드킷): 빠른 재빌드를 위해 권장
# ※ Docker Desktop은 기본적으로 BuildKit 활성화되어 있습니다.
WORKDIR /src/src/MysteryBox.Api
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore

# 게시물 생성 (자체 실행 파일 제외해 이미지 슬림화)
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release -o /out /p:UseAppHost=false

# ================== Runtime stage ==================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# K8s에서 5000 포트 사용, 앱은 http로 리슨(ingress에서 TLS 종료하는 일반 패턴)
ENV ASPNETCORE_URLS=http://0.0.0.0:5000 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 5000
COPY --from=build /out ./

ENTRYPOINT ["dotnet", "MysteryBox.Api.dll"]
