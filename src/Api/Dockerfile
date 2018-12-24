FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Api/Api.csproj src/Api/
RUN dotnet restore src/Api/Api.csproj
COPY . .
WORKDIR /src/src/Api
RUN dotnet build Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Api.dll"]
