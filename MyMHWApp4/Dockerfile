FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyMHWApp4/MyMHWApp4.csproj", "MyMHWApp4/"]
COPY ["MHWApp.Library/MHWApp.Library.csproj", "MHWApp.Library/"]
RUN dotnet restore "MyMHWApp4/MyMHWApp4.csproj"
COPY . .
WORKDIR "/src/MyMHWApp4"
RUN dotnet build "MyMHWApp4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyMHWApp4.csproj" -c Release -o /app/publish

FROM caddy as final
WORKDIR /var/www/web
COPY --from=publish /app/publish/wwwroot .
COPY MyMHWApp4/Caddyfile /etc/caddy/Caddyfile
EXPOSE 80