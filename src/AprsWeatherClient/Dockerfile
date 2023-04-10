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
RUN dos2unix AprsWeatherClient/deploy/analytics.sh
RUN chmod a+rwx AprsWeatherClient/deploy/analytics.sh
RUN AprsWeatherClient/deploy/analytics.sh $ENVIRONMENT
RUN dotnet build AprsWeatherClient/AprsWeatherClient.csproj --configuration Release --no-restore

# Publish
RUN dotnet publish AprsWeatherClient/AprsWeatherClient.csproj --configuration Release --no-build --no-restore -o /app/publish

# Rename Geolocation.js -> geolocation.js to support nginx casting file name to lower
RUN mv /app/publish/wwwroot/_content/Darnton.Blazor.DeviceInterop/js/Geolocation.js /app/publish/wwwroot/_content/Darnton.Blazor.DeviceInterop/js/geolocation.js

# Build final host image
FROM nginx:1.21.6-alpine AS host
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY --from=build /src/AprsWeatherClient/nginx.conf /etc/nginx/nginx.conf
