FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/IdentityServer/IdentityServer.csproj src/IdentityServer/
RUN dotnet restore src/IdentityServer/IdentityServer.csproj
COPY . .
WORKDIR /src/src/IdentityServer
RUN dotnet build IdentityServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IdentityServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
