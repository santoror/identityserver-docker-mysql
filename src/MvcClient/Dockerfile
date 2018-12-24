FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/MvcClient/MvcClient.csproj src/MvcClient/
RUN dotnet restore src/MvcClient/MvcClient.csproj
COPY . .
WORKDIR /src/src/MvcClient
RUN dotnet build MvcClient.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MvcClient.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MvcClient.dll"]
