FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
COPY bin/Release/net8.0/publish/ app/
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN dotnet dev-certs https -v -ep openiddict.pfx -p b40d4664-8e26-4172-b6a8-6a8c8de03ab7

FROM base AS final
WORKDIR /app
COPY --from=build /src .

ENTRYPOINT ["dotnet", "IBLTermocasa.AuthServer.dll"]