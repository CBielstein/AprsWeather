FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14-amd64 AS build
WORKDIR /src

# Restore
COPY AprsWeatherServer.csproj .
RUN dotnet restore

# Build
COPY . .
RUN dotnet build --configuration Release --no-restore

# Publish
RUN dotnet publish --configuration Release --no-build --no-restore -o /app/publish

# Build final host image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14-amd64 as host
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AprsWeatherServer.dll"]
