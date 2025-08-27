# =========== Build ===========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# GitHub 리포를 직접 clone
RUN git clone https://github.com/kil2405/EFCore_Mystery.git .

# Restore & Publish (API 프로젝트 기준)
RUN dotnet restore src/MysteryBox.Api/MysteryBox.Api.csproj
RUN dotnet publish src/MysteryBox.Api/MysteryBox.Api.csproj -c Release -o /out

# =========== Runtime =========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

COPY --from=build /out .
ENTRYPOINT ["dotnet", "MysteryBox.Api.dll"]
