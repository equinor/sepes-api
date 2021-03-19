@echo off
dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools
rm -r %~dp0\Sepes.RestApi.IntegrationTests\TestResults\
rm -r %~dp0\Sepes.Tests\TestResults\

dotnet test .\Sepes.Tests\ --collect:"XPlat Code Coverage" --settings .\Sepes.Tests\unittests.runsettings
tools\reportgenerator.exe -reports:Sepes.Tests/TestResults/**/coverage.cobertura.xml -targetdir:./CodeCoverage/UnitTests -reporttypes:HtmlSummary -title:"Unit Tests"

dotnet test .\Sepes.RestApi.IntegrationTests\ --collect:"XPlat Code Coverage" --settings .\Sepes.RestApi.IntegrationTests\integrationtests.runsettings
tools\reportgenerator.exe -reports:Sepes.RestApi.IntegrationTests/TestResults/**/coverage.cobertura.xml -targetdir:./CodeCoverage/IntegrationTests -reporttypes:HtmlSummary -title:"Integration Tests"

tools\reportgenerator.exe -reports:Sepes.Tests/TestResults/**/coverage.cobertura.xml;Sepes.RestApi.IntegrationTests/TestResults/**/coverage.cobertura.xml -targetdir:./CodeCoverage/Combined -reporttypes:HtmlSummary -title:"All Tests"
start chrome ./CodeCoverage/UnitTests/summary.htm
start chrome ./CodeCoverage/IntegrationTests/summary.htm
start chrome ./CodeCoverage/Combined/summary.htm
pause