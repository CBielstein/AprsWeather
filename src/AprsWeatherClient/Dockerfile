FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.14-amd64 AS build
ARG ENVIRONMENT=Production
WORKDIR /src

# Restore
COPY AprsWeatherClient.csproj .
RUN dotnet restore

# Build
COPY . .
RUN sed -i "s/%ENVIRONMENT%/$ENVIRONMENT/g" wwwroot/index.html
RUN cat wwwroot/index.html
RUN dotnet build --configuration Release --no-restore

# Publish
RUN dotnet publish --configuration Release --no-build --no-restore -o /app/publish

# Build final host image
FROM nginx:1.21.6-alpine AS host
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY --from=build /src/nginx.conf /etc/nginx/nginx.conf
