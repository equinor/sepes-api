@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
rem dotnet test .\Sepes.RestApi.IntegrationTests\ /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/IntegrationTest /p:CoverletOutputFormat=cobertura 
dotnet test .\Sepes.Tests\ /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/UnitTest /p:CoverletOutputFormat=cobertura 
rem tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml;CodeCoverage/**/IntegrationTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlInline
tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
start chrome ./CodeCoverage/Report/index.htm
pause