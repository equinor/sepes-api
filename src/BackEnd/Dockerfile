FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
# TODO: Copy only csproj
COPY . .
RUN dotnet restore Sepes.RestApi/Sepes.RestApi.csproj

# Copy everything else and build
# TODO: Copy rest of files
# COPY . ./
RUN dotnet publish Sepes.RestApi -c Release -o Sepes.RestApi/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/Sepes.RestApi/out .
ENTRYPOINT ["dotnet", "Sepes.RestApi.dll", "--environment=Development"]

# Declare the port the API will communicate through. Must be ran with -p command to actually ezpose ports
EXPOSE 5001