# sepes-api
![Unit Tests](https://github.com/equinor/sepes-api/workflows/Unit%20Tests/badge.svg?event=push)

A platform that allows vendors prove their solutions on your data in a sandbox


## Making commits

We use [commitizen](http://commitizen.github.io/cz-cli) (and [conventional-changelog](https://github.com/conventional-changelog/conventional-changelog)) to make our commits. Tip:  run "npx git-cz" to make commits. 

We use (semver)[https://semver.org] to choose if a change is MAJOR, MINOR or a PATCH:

- MAJOR version when you make incompatible API changes,
- MINOR version when you add functionality in a backwards compatible manner, and
- PATCH version when you make backwards compatible bug fixes.

## Setup local dev environment

- [Use this guide](./docs/developers/setup-dev-environment.md)

## System architecture

C4 context diagram:

- ![C1 model](/docs/platform/C4ContextDiagram.svg)


C4 container diagram:

- ![C2 model](/docs/platform/C4ContainerDiagram.svg)

[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg)](http://commitizen.github.io/cz-cli/)
