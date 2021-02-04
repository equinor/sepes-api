@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
dotnet test .\Sepes.RestApi.IntegrationTests\ /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/IntegrationTest /p:CoverletOutputFormat=cobertura 
dotnet test .\Sepes.Tests\ /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/UnitTest /p:CoverletOutputFormat=cobertura 
tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml;CodeCoverage/**/IntegrationTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HTML;HTMLSummary
start chrome ./CodeCoverage/Report/index.htm
pause