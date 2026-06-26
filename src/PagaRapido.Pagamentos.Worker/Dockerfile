FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/PagaRapido.Shared/PagaRapido.Shared.csproj src/PagaRapido.Shared/
COPY src/PagaRapido.Pagamentos.Worker/PagaRapido.Pagamentos.Worker.csproj src/PagaRapido.Pagamentos.Worker/
RUN dotnet restore src/PagaRapido.Pagamentos.Worker/PagaRapido.Pagamentos.Worker.csproj
COPY src/PagaRapido.Shared/ src/PagaRapido.Shared/
COPY src/PagaRapido.Pagamentos.Worker/ src/PagaRapido.Pagamentos.Worker/
RUN dotnet publish src/PagaRapido.Pagamentos.Worker/PagaRapido.Pagamentos.Worker.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PagaRapido.Pagamentos.Worker.dll"]
