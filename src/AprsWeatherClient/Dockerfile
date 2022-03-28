FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14-amd64 AS build
ARG ENVIRONMENT=Production
WORKDIR /src

# Restore
COPY AprsWeatherClient/AprsWeatherClient.csproj AprsWeatherClient/
COPY AprsWeatherShared/AprsWeatherShared.csproj AprsWeatherShared/
RUN dotnet restore AprsWeatherClient/AprsWeatherClient.csproj

# Build
COPY AprsWeatherClient/ AprsWeatherClient/
COPY AprsWeatherShared/ AprsWeatherShared/

# Build
COPY . .
RUN sed -i "s/%ENVIRONMENT%/$ENVIRONMENT/g" AprsWeatherClient/wwwroot/index.html
RUN dotnet build AprsWeatherClient/AprsWeatherClient.csproj --configuration Release --no-restore

# Publish
RUN dotnet publish AprsWeatherClient/AprsWeatherClient.csproj --configuration Release --no-build --no-restore -o /app/publish

# Build final host image
FROM nginx:1.21.6-alpine AS host
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY --from=build /src/AprsWeatherClient/nginx.conf /etc/nginx/nginx.conf
