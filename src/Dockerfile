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

RUN addgroup --gid 1001 radix-non-root-group
RUN adduser --uid 1001 --gid 1001 radix-non-root-user

USER 1001

EXPOSE 3000
ENTRYPOINT ["dotnet", "Sepes.RestApi.dll"]