# Development environment setup for Sepes BackEnd

## Prerequisites:

To be able to run and develop for this project there are a some runtimes that need to be installed.

* [Dotnet Core SDK 3.0](https://dotnet.microsoft.com/download)

* [Microsoft SQL Server](https://www.microsoft.com/nb-no/sql-server/sql-server-downloads)

### Azure
These services are required
* Azure AD App Registration (also generate a personal client secret)
* Subscribtions
* Application Insight


## Setup configuration
Sepes use various sources for configuration values, depending on what environment you are setting up (local dev without debugging, with debugging, in cloud etc) 

1. appsettings.json
2. appsettings.Development.json
3. Environment Variables
    * Locally: Supplied from docker-compose
    * In cloud: From the hosting technology
4. Key vault

### Debugging in Visual Studio
If you want to run the Backend in Visual Studio, you can get away with appsettings.json and appsettings.Development.json
appsettings.Development.json is ignored by git, so you'll have to create your own copy


### Running using docker-compose
If you want to run the Backend using Docker Compose, you can create a docker-compose.yml file in the root folder of the repository
docker-compose.yml is ignored by git, so you'll have to create your own copy

###



All values bellow are written in without quotation marks
```
SEPES_NAME=
```
This is the name that will be used to create resources within azure. Do not include spaces.
```
SEPES_TENANT_ID=            
```
```
SEPES_CLIENT_ID=            
```
```
SEPES_CLIENT_SECRET=        
```
```
SEPES_INSTRUMENTATION_KEY=  
```
This is found in the Overview tab foun in the Application Insights service created in Azure.
```
SEPES_SUBSCRIPTION_ID=      
```
This is the Subscrition ID of the subscribtion sepes will use for its operation
```
SEPES_MSSQL_CONNECTION_STRING=
```
Needs to be in following format: 
```
Data Source={ip or url to server};Initial Catalog={name of catalog};User ID={userID};Password={password}
```
```
SEPES_HTTP_ONLY=false
```
This should only be set to true if you are intending to run SEPES behind some other proxy that will provide encryption, like for example Docker.

    
## Setup database
TODO: Document how to use migrations
* Option 1: Use SQL Query
    * Create or have an SQL Server
    * Open a connection to SQL Server
    * Use the query file or copy its contents into the management softwares query editor.

* Option 2:
    * Use the full server copy and import it into Microsoft SQL Server Management Studio.
    * You need an existing sql server on Azure you can target for deployment
    * Use Microsoft SQL Server Management Studio to locally import.
    * Right click database and select Tasks>Deploy to azure

## Setup monitoring service.
* Create an Application Insights instance for SEPES
* In the overview tab copy the "Instrumentation Key" and paste into the .env file as described above.
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