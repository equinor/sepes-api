# sepes-api

![Unit Tests](https://github.com/equinor/sepes-api/workflows/Unit%20Tests/badge.svg?event=push)
![CodeQL](https://github.com/equinor/sepes-api/workflows/CodeQL/badge.svg?event=push)
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/HansKristianVikOlsen/ad55c36852976fbafa4c2198e0deecaf/raw/test.json)

A platform that allows vendors prove their solutions on your data in a sandbox

## Making commits

Use conventional commits as described in https://github.com/equinor/sdscoredev-handbook#git-commits

Valid `scope`-s for Sepes are:

- study
- sandbox
- dataset

## Setup local dev environment

TODO: Fix this, old guide is gone

## Tests

The project Sepes.Tests contains Unit Tests, and the Sepes.RestApi.IntegrationTests contains the Integration Tests
xUnit is used as test framework.
For Integration tests, WebApplicationFactory is used, calling the Api Controllers and their dependencies through actual HTTP requests.

### How to run tests

#### Prerequisites

- Integration tests uses a SQL database, and it should NOT be the same as the development database, because it will be wiped before every test run.
  - There is currently a bug, that requires one to specify the connection string for the integration test database, in both Sepes.RestApi and Sepes.RestApi.IntegrationTests
  - Use the config key: ConnectionStrings:SqlDatabaseIntegrationTests to specify the connectionString for the Integration test database.

#### Best option: Run from Visual Studio

Open the "Test Explorer"-Window and click the "Run All Tests In View"

#### Console

- To run both unit and integration tests, navigate to the solution folder and run the "dotnet test" command from there
- To run only unit tests or integration tests, navigate to their respective project folder and run "dotnet test"

### How to generate test coverage report

Go to the solution folder and run the testcoverage.bat file.
It will produce a coverage file and open it in the browser.

[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg)](http://commitizen.github.io/cz-cli/)
