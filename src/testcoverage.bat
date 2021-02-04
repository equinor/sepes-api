@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
rem dotnet test .\Sepes.RestApi.IntegrationTests\ /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/IntegrationTest /p:CoverletOutputFormat=cobertura 
rem dotnet test .\Sepes.Tests\ --settings .\Sepes.Tests.Common/test.runsettings /p:CollectCoverage=true /p:CoverletOutput=../CodeCoverage/UnitTest /p:CoverletOutputFormat=cobertura 
dotnet test .\Sepes.Tests\ /p:CollectCoverage=true /p:Exclude=\"[Sepes.Tests.Common]*,[*]Sepes.Infrastructure.Dto.*,[*]Sepes.Infrastructure.Model.*,[*]Sepes.Infrastructure.Constants.*,[*]Sepes.Infrastructure.Exceptions.*,[*]Sepes.Infrastructure.Migrations.*,[*]Sepes.Infrastructure.Extensions.*,[*]Microsoft.AspNetCore.Authentication.* \" /p:CoverletOutput=../CodeCoverage/UnitTest /p:CoverletOutputFormat=cobertura 
rem tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml;CodeCoverage/**/IntegrationTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlInline
tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
start chrome ./CodeCoverage/Report/summary.htm
pause