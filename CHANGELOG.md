# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

### [0.4.2](https://github.com/equinor/sepes-api/compare/0.4.1...0.4.2) (2021-01-05)


### Features

* **dataset:** deny connections and make available to sandbox ([#454](https://github.com/equinor/sepes-api/issues/454)) ([844c710](https://github.com/equinor/sepes-api/commit/844c7102ac577a8135dd4a1fe49f8460ed36575a)), closes [#458](https://github.com/equinor/sepes-api/issues/458)


### Bug Fixes

* **auth:** fixed login redirect issue. Not requiring User.Read scope on signin anymore ([1f74345](https://github.com/equinor/sepes-api/commit/1f74345f87e49744738425dd4c63be8e88121f36))
* **dataset:** increased dataset max upload size ([23be629](https://github.com/equinor/sepes-api/commit/23be62957e9b50fdae780dea438dee3e6abfec35))
* **rbac:** admin was still missing some permissions that only he shold have ([37087f1](https://github.com/equinor/sepes-api/commit/37087f1f1beffccd05739386995acd5ef1080913))
* **sandbox:** sort sizes by price with lowest cost first ([#462](https://github.com/equinor/sepes-api/issues/462)) ([f56f01d](https://github.com/equinor/sepes-api/commit/f56f01d9402cc78d257452e9a4aea9e811a17b50)), closes [#423](https://github.com/equinor/sepes-api/issues/423)
* **worker:** decreased time taken for VM creation to start ([#463](https://github.com/equinor/sepes-api/issues/463)) ([f0fe1f7](https://github.com/equinor/sepes-api/commit/f0fe1f74fef8c320d871f3a203aa060891d52850))

### [0.4.1](https://github.com/equinor/sepes-api/compare/0.4.0...0.4.1) (2020-12-10)

## [0.4.0](https://github.com/equinor/sepes-api/compare/0.3.0...0.4.0) (2020-12-10)


### ⚠ BREAKING CHANGES

* Study details response does not contain resultsAndLearnings property

### Features

* feat(dataset): implemented file upload for study specific datasets


### Bug Fixes

* fix: now able to get study even though logo functionality is failing

previously, all endpoints for study was failing if something went wrong with logo

* refactor: moved dataset dtos into separate folder and changed namespace

* fix: study dataset endpoint now returning url to storage account

* fix: create storage account improve logging

* refactor: dataset file response: renamed SizeInBytes to bytes

* feat(studyspecificdataset): now creating resource group, storage account and accepting upload

* refactor: study specific datasets: prepared the ground for allowed ips, but left out for now

* refactor(datasets): improving service architecture . Separate service for datasets cloud resources

makes it easier to mock and test these components

* fix: added automapper config for azurestorageaccountdto

* test(dataset): fixed existing tests after refactor, and added a few new

* **rbac:** added missing permissions for admin ([#445](https://github.com/equinor/sepes-api/issues/445)) ([ea8a189](https://github.com/equinor/sepes-api/commit/ea8a189d4b8ee9a576ccefc685227bfac9e42a5b))
* **study:** delete failed when no dataset resource group was set ([#444](https://github.com/equinor/sepes-api/issues/444)) ([ff30438](https://github.com/equinor/sepes-api/commit/ff30438fa862549897018be534b729462e7078d7))


* feat(studyspecificdataset)/implement file upload (#443) ([6dcd4c4](https://github.com/equinor/sepes-api/commit/6dcd4c4663978a94f41a3739f81c29677dcf7227)), closes [#443](https://github.com/equinor/sepes-api/issues/443)

## [0.3.0](https://github.com/equinor/sepes-api/compare/0.2.5...0.3.0) (2020-12-02)


### ⚠ BREAKING CHANGES

* three endpoints described above are replaced by endpoints not requiring the
studies/studyId part

### Features

* **vm:** new endpoint for external link to virtual machine ([#422](https://github.com/equinor/sepes-api/issues/422)) ([1ae8097](https://github.com/equinor/sepes-api/commit/1ae8097dbf99200087502242a4192e3a8354826d))


### Bug Fixes

* **rbac:** added role for employee vs external user. more unit tests.… ([#440](https://github.com/equinor/sepes-api/issues/440)) ([12f44a6](https://github.com/equinor/sepes-api/commit/12f44a6c0f4ab97917f7c0eb9bd69ce3eb9e044f))
* **study:** returning the detailed response for CreateStudy, UpdateLogo and UpdateStudyDetails ([f5a4fb1](https://github.com/equinor/sepes-api/commit/f5a4fb1fef220032e5f6a8f83147e029d4bd93ea))
* (SandboxResourceController): methods missing "/api/" prefix. ([#436](https://github.com/equinor/sepes-api/issues/436)) ([19d3ff9](https://github.com/equinor/sepes-api/commit/19d3ff927823eea97ba24b80a30552c87a351e2c))
* renamed dataset properties in permissions response. Also now using rbac engine ([#432](https://github.com/equinor/sepes-api/issues/432)) ([895b713](https://github.com/equinor/sepes-api/commit/895b713f5607c68dfaca133a2b4ca3bb93cf393b))
* **sandbox creation:** rule priority for basic nsg rule was to high, now set to 4050 ([946009f](https://github.com/equinor/sepes-api/commit/946009fbe688366f5058df1270331eec6e1e6290))
* **vm:** vm extended info endpoint, renamed MemoryInDb to MemoryGB ([#424](https://github.com/equinor/sepes-api/issues/424)) ([939cca7](https://github.com/equinor/sepes-api/commit/939cca7d7520f74defca8edd3feaac245a85fd31))


* removed sandbox endpoints that were simplified to not include/study/studyId part ([#437](https://github.com/equinor/sepes-api/issues/437)) ([cde78c0](https://github.com/equinor/sepes-api/commit/cde78c0d46cfcea20467f3237dad6342e620030a))

### [0.2.5](https://github.com/equinor/sepes-api/compare/0.2.4...0.2.5) (2020-11-19)

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
