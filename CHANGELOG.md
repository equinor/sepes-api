# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

### [0.2.4](https://github.com/equinor/sepes-api/compare/0.2.3...0.2.4) (2020-11-19)


### Bug Fixes

* **study details:** missing include causing mapping error for StudyDatasets > SandboxDataset ([#414](https://github.com/equinor/sepes-api/issues/414)) ([9ae327c](https://github.com/equinor/sepes-api/commit/9ae327c3ccfb1452e32928b2b7d9a09a45003dc1))
* **vm rules:** fixed issue where vm creation and update lost got off track ([#416](https://github.com/equinor/sepes-api/issues/416)) ([a8025b0](https://github.com/equinor/sepes-api/commit/a8025b08abd6ea77d2a542207aa5e9ca22e0c27e))
* **vm rules:** now keeping track of priorities, asking azure when creating rules ([#413](https://github.com/equinor/sepes-api/issues/413)) ([485ad82](https://github.com/equinor/sepes-api/commit/485ad82b8500dd5c1bba3d6621705687b992f9bc))

### [0.2.3](https://github.com/equinor/sepes-api/compare/0.2.2...0.2.3) (2020-11-18)


### Features

* **sandbox:** add link to cost analysis ([#412](https://github.com/equinor/sepes-api/issues/412)) ([2c42d3d](https://github.com/equinor/sepes-api/commit/2c42d3d399ce797427ffd97d949094aaade1be1e)), closes [#342](https://github.com/equinor/sepes-api/issues/342)


### Bug Fixes

* **azurequeueservicetest:** was not building, commented out ([#370](https://github.com/equinor/sepes-api/issues/370)) ([10343ef](https://github.com/equinor/sepes-api/commit/10343ef9ad8237ed8fa57b167bcbca2653ada9ba))
* **sandbox creation:** ensuring study and sandbox name part of resour… ([#401](https://github.com/equinor/sepes-api/issues/401)) ([fe26189](https://github.com/equinor/sepes-api/commit/fe26189c902994850f36bcd0dbf4198a3a4f521d))
* **sandbox resource list:** fixed when status is showing create failed for deleted resources ([0766619](https://github.com/equinor/sepes-api/commit/07666193158f18f18a2373d1455f71b47595b1bc))
* **study create:** was failing due to missing permission check ([#376](https://github.com/equinor/sepes-api/issues/376)) ([7a01d95](https://github.com/equinor/sepes-api/commit/7a01d95b3d8cc05a834859d5b243439c961cd645))
* **studyDetails:** added which sandboxes a given datasets is used in ([#403](https://github.com/equinor/sepes-api/issues/403)) ([d960358](https://github.com/equinor/sepes-api/commit/d960358bab05d8cc9b3a2c02281698375c831adb))
* **virtual machine:** resolve empty size lookup for some regions ([#402](https://github.com/equinor/sepes-api/issues/402)) ([b628869](https://github.com/equinor/sepes-api/commit/b628869538e1d61e883ce9caef43fbfb09f24865))
* **vm rules:** returning empty list instead of null, when no rules exist ([#379](https://github.com/equinor/sepes-api/issues/379)) ([7bf027d](https://github.com/equinor/sepes-api/commit/7bf027d37a97c64408108e6dbcd3ee33446d8de8))

### [0.2.2](https://github.com/equinor/sepes-api/compare/0.2.1...0.2.2) (2020-11-12)

### [0.2.1](https://github.com/equinor/sepes-api/compare/0.2.0...0.2.1) (2020-11-11)

## 0.2.0 (2020-11-11)
