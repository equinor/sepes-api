@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
rm -r %~dp0\Sepes.RestApi.IntegrationTests\TestResults\
rm -r %~dp0\Sepes.Tests\TestResults\
dotnet test .\Sepes.RestApi.IntegrationTests\ --collect:"XPlat Code Coverage" --settings coverage.runsettings
dotnet test .\Sepes.Tests\ --collect:"XPlat Code Coverage" --settings coverage.runsettings
tools\reportgenerator.exe -reports:Sepes.Tests/TestResults/**/coverage.cobertura.xml;Sepes.RestApi.IntegrationTests/TestResults/**/coverage.cobertura.xml -targetdir:./CodeCoverage/Report -reporttypes:HtmlSummary
start chrome ./CodeCoverage/Report/summary.htm
pause