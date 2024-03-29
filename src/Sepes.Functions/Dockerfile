FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# WORKDIR /src

COPY Sepes.Infrastructure /src/Sepes.Infrastructure
COPY Sepes.Common /src/Sepes.Common
COPY Sepes.Provisioning /src/Sepes.Provisioning
COPY Sepes.Azure /src/Sepes.Azure
COPY Sepes.Functions /src/Sepes.Functions
RUN cd /src && \
    ls
RUN cd /src/Sepes.Functions && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish Sepes.Functions.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:4.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

COPY --from=build-env ["/home/site/wwwroot", "/home/site/wwwroot"]


# Copy csproj and restore as distinct layers
# TODO: Copy only csproj
# COPY . .
# RUN dotnet restore Sepes.Functions/Sepes.Functions.csproj

# Copy everything else and build
# TODO: Copy rest of files
# COPY . ./
# RUN dotnet publish Sepes.Functions -c Release -o Sepes.Functions/out

# Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build-env /app/Sepes.Functions/out .
# ENTRYPOINT ["dotnet", "Sepes.Functions.dll", "--environment=Development"]

# Declare the port the API will communicate through. Must be ran with -p command to actually ezpose ports
# EXPOSE 80