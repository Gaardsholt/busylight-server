FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

# Create user
RUN groupadd -g 2000 app_user && \
  useradd -m -u 2000 -g app_user app_user

# Use user
USER app_user

ENTRYPOINT ["dotnet", "busylight-server.dll"]
