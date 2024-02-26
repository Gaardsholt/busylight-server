FROM mcr.microsoft.com/dotnet/sdk:8.0.101@sha256:1b99a629b4f55a496db835d9c3347737aa4a1d995d43ffe5ab92d04bee69adeb AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0@sha256:894c9f49ae9a72b64e61ef6071a33b6b616d0cf48ef25c83c4cf26d185f37565
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS http://*:5000

# Create user
RUN groupadd -g 2000 app_user && \
  useradd -m -u 2000 -g app_user app_user

# Use user
USER app_user

EXPOSE 5000
ENTRYPOINT ["dotnet", "busylight-server.dll"]
