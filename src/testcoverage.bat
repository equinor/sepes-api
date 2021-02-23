@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
rm -r %~dp0\Sepes.RestApi.IntegrationTests\TestResults\
dotnet test .\Sepes.RestApi.IntegrationTests\ --collect:"XPlat Code Coverage" --settings integrationtests.runsettings
rem dotnet test .\Sepes.Tests\ /p:CollectCoverage=true /p:Exclude=\"[Sepes.Tests.Common]*,[*]Sepes.Infrastructure.Dto.*,[*]Sepes.Infrastructure.Response.*,[*]Sepes.Infrastructure.Service.Azure.*,[*]Sepes.Infrastructure.Model.*,[*]Sepes.Infrastructure.Constants.*,[*]Sepes.Infrastructure.Exceptions.*,[*]Sepes.Infrastructure.Migrations.*,[*]Sepes.Infrastructure.Extensions.*,[*]Microsoft.AspNetCore.Authentication.* \" /p:CoverletOutput=../CodeCoverage/UnitTest /p:CoverletOutputFormat=cobertura 
rem tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml;CodeCoverage/**/IntegrationTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
rem tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
rem tools\reportgenerator.exe -reports:CodeCoverage/**/IntegrationTest.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
tools\reportgenerator.exe -reports:CodeCoverage/**/UnitTest.cobertura.xml;Sepes.RestApi.IntegrationTests/TestResults/**/coverage.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
start chrome ./CodeCoverage/Report/summary.htm
pause