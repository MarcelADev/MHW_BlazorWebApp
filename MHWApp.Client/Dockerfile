FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MHWApp.Client/MHWApp.Client.csproj", "MHWApp.Client/"]
COPY ["MHWApp.Library/MHWApp.Library.csproj", "MHWApp.Library/"]
RUN dotnet restore "MHWApp.Client/MHWApp.Client.csproj"
COPY . .
WORKDIR "/src/MHWApp.Client"
RUN dotnet build "MHWApp.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MHWApp.Client.csproj" -c Release -o /app/publish

FROM caddy as final
WORKDIR /var/www/web
COPY --from=publish /app/publish/wwwroot .
COPY MHWApp.Client/Caddyfile /etc/caddy/Caddyfile
EXPOSE 80