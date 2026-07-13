FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:fdac9ba57a38ffaa6494b93de33983644c44d9e491e4e312f35ddf926c55a073 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0@sha256:7c4246c1c384319346d45b3e24a10a21d5b6fc9b36a04790e1588148ff8055b0
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
