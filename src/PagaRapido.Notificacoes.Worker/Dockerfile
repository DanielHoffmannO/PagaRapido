FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/PagaRapido.Shared/PagaRapido.Shared.csproj src/PagaRapido.Shared/
COPY src/PagaRapido.Notificacoes.Worker/PagaRapido.Notificacoes.Worker.csproj src/PagaRapido.Notificacoes.Worker/
RUN dotnet restore src/PagaRapido.Notificacoes.Worker/PagaRapido.Notificacoes.Worker.csproj
COPY src/PagaRapido.Shared/ src/PagaRapido.Shared/
COPY src/PagaRapido.Notificacoes.Worker/ src/PagaRapido.Notificacoes.Worker/
RUN dotnet publish src/PagaRapido.Notificacoes.Worker/PagaRapido.Notificacoes.Worker.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PagaRapido.Notificacoes.Worker.dll"]
