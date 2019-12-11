Prerequisites:

To be able to run and develop for this project there are a some runtimes that need to be installed.

Dotnet Core SDK 3.0 https://dotnet.microsoft.com/download

Node.js https://nodejs.org/en/

Microsoft SQL Server Developer https://www.microsoft.com/nb-no/sql-server/sql-server-downloads


Add dependencies

In both the Sepes.RestApi and Sepes.RestApi.Test folder run "dotnet restore"

In the FrontEnd folder you need to run "npm install"


Setup config:

You can get the config values from the following places

SEPES_NAME=sepes-dev        
This is the name that will be used to create resources within azure. Make sure to not use - instead of spaces

SEPES_TENANT_ID=            

SEPES_CLIENT_ID=            

SEPES_CLIENT_SECRET=        

SEPES_INSTRUMENTATION_KEY=  

SEPES_SUBSCRIPTION_ID=      

SEPES_MSSQL_CONNECTION_STRING=
Needs to be in following format: 

SEPES_HTTP_ONLY=false
This should only be set to true if you are intending to run SEPES behind some other proxy that will provide encryption, like for example Docker.


Common errors:

Error "Failed to load resource: net::ERR_CERT_AUTHORITY_INVALID"
Solution:
            Try running "dotnet dev-certs https --clean" then  "dotnet dev-certs https --trust" to reinstall the dev certificate