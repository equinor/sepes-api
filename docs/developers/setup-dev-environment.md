# Development environment setup for Sepes API

## Prerequisites:

### Azure Resources
* Azure AD App Registration (also generate a separate client secret for each developer)
* Subscriptions for creating resources (Sandbox, Storage etc)
* Application Insights
* Key vault (optional for local development) and relevant access policies/FW rules

### Software

To be able to run and develop for this project there are a some runtimes that need to be installed.

* [Dotnet Core SDK 3.X](https://dotnet.microsoft.com/download)

### Dev Tools
Some are optional and some depends on taste

* [Visual Studio and/or Visual Studio Code](https://visualstudio.microsoft.com/)

* [Docker for Windows and Docker Compose](docker.com) (Needed for running Sepes in a container locally)

* [Microsoft SQL Server](https://www.microsoft.com/nb-no/sql-server/sql-server-downloads) and SQL Server Management Studio

* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest)

* Azure Container Registry (ACR) (Or any other CR)
    * You can push Sepes as containers here, so that Docker Compose can pull from there when running locally




## Setup configuration
Sepes use various sources for configuration values, depending on what environment you are setting up (local dev without debugging, with debugging, in cloud etc).

The config values are picked up in the order listed below. Eg, if the same value is supplied from both appsettings.json, and in Key Vault, the value from Key Vault will apply, because it comes last, and therefore overwrites any previous ones.

1. appsettings.json
2. appsettings.Development.json (if present)
3. Environment Variables
    * Locally: Usually supplied from docker-compose
    * In cloud: From the hosting technology
4. Key vault

### Debugging in Visual Studio
If you want to run the Backend in Visual Studio, create your personal appsettings.Development.json.
This file is ignored by git, so you'll have to create your own copy. Use this [template](appsettings.Development_SAMPLE.json)


### Running using docker-compose
If you want to run the Backend using Docker Compose, you can create a docker-compose.yml file in the root folder of the repository
docker-compose.yml is ignored by git, so you'll have to create your own copy. Use this [template](docker-compose_SAMPLE.yml)

    
## Setup database
Sepes use EF Core Code First with Migrations. 
1. Manually create an empty database on your SQL Server
2. Configure your connection string to target that relevant server/DB
3. Run sepes API. It will automatically run the migrations against the DB (creating tables, etc)


## Setup monitoring service.
* Create an Application Insights instance for SEPES
* In the overview tab, copy the "Instrumentation Key" and paste into the your personal config file.
* Boot up an instance of SEPES to verify that it logs correctly to Application Insights

## Common issues:

Error: 
```
"Failed to load resource: net::ERR_CERT_AUTHORITY_INVALID"
```
Solution:
Run the below command
```
dotnet dev-certs https --clean
```
then after that command succesfully executes, run
```
dotnet dev-certs https --trust
```
This should reinstal the dev-certificate