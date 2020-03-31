FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY /Sepes.RestApi/*.csproj .
RUN dotnet restore

# Copy everything else and build
COPY /Sepes.RestApi/. .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Sepes.RestApi.dll", "--environment=Development"]

# Declare the port the API will communicate through. Must be ran with -p command to actually ezpose ports
EXPOSE 5001
