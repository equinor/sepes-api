# Dev environment setup

## Prerequisites:

To be able to run and develop for this project there are a some runtimes that need to be installed.

Dotnet Core SDK 3.0 https://dotnet.microsoft.com/download

Node.js https://nodejs.org/en/

Microsoft SQL Server Developer https://www.microsoft.com/nb-no/sql-server/sql-server-downloads

Microsoft SQL Server

### Azure
These services are required
-AD app 
-Subscribtion
-Insight



## Add dependencies

In both the Sepes.RestApi and Sepes.RestApi.Test folder run the command "dotnet restore"

In the FrontEnd folder you need to run the commdand "npm install"


## Setup config:

You can get the config values from the following places

SEPES_NAME=sepes-dev
This is the name that will be used to create resources within azure. Make sure to not use spaces

SEPES_TENANT_ID=            

SEPES_CLIENT_ID=            

SEPES_CLIENT_SECRET=        

SEPES_INSTRUMENTATION_KEY=  
THis is found on the Application Insights service created on Azure for logging. It can be found in the Overview tab.

SEPES_SUBSCRIPTION_ID=      
This is the Subscrition ID of the subscribtion sepes will use for its operation

SEPES_MSSQL_CONNECTION_STRING=
Needs to be in following format: "Data Source={ip or url to server};Initial Catalog={name of catalog};User ID={userID};Password={password}"

SEPES_HTTP_ONLY=false
This should only be set to true if you are intending to run SEPES behind some other proxy that will provide encryption, like for example Docker.

    
## Setup database
* Option 1: Use SQL Query
    * Create or have an SQL Server
    * Open a connection to SQL Server
    * Use the query file or copy its contents into the management softwares query editor.

* Option 2:
    * Use the full server copy and import it into Microsoft SQL Server Management Studio.
    * You need an existing sql server on Azure you can target for deployment
    * Use Microsoft SQL Server Management Studio to locally instal and test the full file.
    * Right click database and select Tasks>Deploy to azure

## Setup monitoring service.
In Azure create an Application Insights instance for SEPES and add its instrumentation key to the .env file as described above.

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