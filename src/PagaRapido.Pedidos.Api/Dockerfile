FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/PagaRapido.Shared/PagaRapido.Shared.csproj src/PagaRapido.Shared/
COPY src/PagaRapido.Pedidos.Api/PagaRapido.Pedidos.Api.csproj src/PagaRapido.Pedidos.Api/
RUN dotnet restore src/PagaRapido.Pedidos.Api/PagaRapido.Pedidos.Api.csproj
COPY src/PagaRapido.Shared/ src/PagaRapido.Shared/
COPY src/PagaRapido.Pedidos.Api/ src/PagaRapido.Pedidos.Api/
RUN dotnet publish src/PagaRapido.Pedidos.Api/PagaRapido.Pedidos.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PagaRapido.Pedidos.Api.dll"]
