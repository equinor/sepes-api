# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

### [0.4.20](https://github.com/equinor/sepes-api/compare/0.4.19...0.4.20) (2021-06-03)


### Features

* **study:** validate wbs on save ([#680](https://github.com/equinor/sepes-api/issues/680)) ([adf647e](https://github.com/equinor/sepes-api/commit/adf647ea9b13befab06d835594b496f8b68c0a33)), closes [#681](https://github.com/equinor/sepes-api/issues/681)
* add wbs validation from external api ([#677](https://github.com/equinor/sepes-api/issues/677)) ([f607cf9](https://github.com/equinor/sepes-api/commit/f607cf9e976888ec6e57297ff062181a57d3ba6a))

### [0.4.19](https://github.com/equinor/sepes-api/compare/0.4.18...0.4.19) (2021-05-19)


### Bug Fixes

* **function:** updated managed by ids for dev and prod ([#663](https://github.com/equinor/sepes-api/issues/663)) ([03d44d6](https://github.com/equinor/sepes-api/commit/03d44d6f5dfbc51507b454dd292de166f49fc01c))
* **logging:** clean up appi event ids ([#670](https://github.com/equinor/sepes-api/issues/670)) ([869ccca](https://github.com/equinor/sepes-api/commit/869ccca26774791556506cae752d37740ba21cb6))
* **participant:** remove created by filter on existing role assignments before add ([#667](https://github.com/equinor/sepes-api/issues/667)) ([47c8663](https://github.com/equinor/sepes-api/commit/47c8663c6add37b6f2b6a2399f59a70af8c49888))
* **study:** add and remove participant did not save ([a78d8e4](https://github.com/equinor/sepes-api/commit/a78d8e47abded38395a065bc6a60ebf831317271))
* **study:** fix slow study close due to include ([#668](https://github.com/equinor/sepes-api/issues/668)) ([e1628fd](https://github.com/equinor/sepes-api/commit/e1628fd1a738c26d352da41e2ee7f496b1208929))

### [0.4.18](https://github.com/equinor/sepes-api/compare/0.4.17...0.4.18) (2021-04-29)


### Bug Fixes

* **api:** enable cors for only configured domains, used to be open for all ([#652](https://github.com/equinor/sepes-api/issues/652)) ([06bd5ea](https://github.com/equinor/sepes-api/commit/06bd5ea0031c34413f268fccb33e90b5e0f00562))
* **auth:** convert to PKCE auth flow ([#656](https://github.com/equinor/sepes-api/issues/656)) ([8cd0b19](https://github.com/equinor/sepes-api/commit/8cd0b1959e5c64f1e780ede969d5e13049c0e62d))
* **dataset:** add properties key and last modified to filelist ([#655](https://github.com/equinor/sepes-api/issues/655)) ([db73634](https://github.com/equinor/sepes-api/commit/db73634a13b2b548da2e4ca678242bb57bc34096))
* **dataset:** delete not working ([#654](https://github.com/equinor/sepes-api/issues/654)) ([32f6de9](https://github.com/equinor/sepes-api/commit/32f6de9ebea4ce7819552fb841e16c520f5b6992))
* **keyvault:** clean old passwords, changed created filter to use utc ([a31feb8](https://github.com/equinor/sepes-api/commit/a31feb8d371d4d3432be885aa11e95058ff80e56))
* **sandbox:** attempt to prevent fail of role assignment update task ([#649](https://github.com/equinor/sepes-api/issues/649)) ([fe12b58](https://github.com/equinor/sepes-api/commit/fe12b58e70bb7e7d6246303139d779c33b578a73))
* **study:** now sponsor and sponsor rep are allowed to soft delete st… ([#653](https://github.com/equinor/sepes-api/issues/653)) ([1801f72](https://github.com/equinor/sepes-api/commit/1801f72cc9dbce3fd0c5ed6761c39589d0f2f986))
* prevent timeout when getting studies ([#645](https://github.com/equinor/sepes-api/issues/645)) ([44e329b](https://github.com/equinor/sepes-api/commit/44e329b13348fd87ec306792f4199c3f4444d24f))

### [0.4.17](https://github.com/equinor/sepes-api/compare/0.4.16...0.4.17) (2021-04-07)

### [0.4.16](https://github.com/equinor/sepes-api/compare/0.4.15...0.4.16) (2021-04-07)


### Bug Fixes

* **sandbox:** add correct dataset restrictions text ([3b3afd8](https://github.com/equinor/sepes-api/commit/3b3afd839d85af59949cd9c4ae59eb1829e66c68))
* **startup:** better logging for empty db con string ([d40427f](https://github.com/equinor/sepes-api/commit/d40427f48a5337d24ec74f692a0ca7c914a65ee0))

### [0.4.15](https://github.com/equinor/sepes-api/compare/0.4.14...0.4.15) (2021-03-26)


### Features

* enabled norwaywest, europenorth and europewest ([#631](https://github.com/equinor/sepes-api/issues/631)) ([10eab76](https://github.com/equinor/sepes-api/commit/10eab76dcd99791061e6bf9cc5b544bfba302580))


### Bug Fixes

* **vm:** filtered away zrs disks ([#634](https://github.com/equinor/sepes-api/issues/634)) ([08e3ca3](https://github.com/equinor/sepes-api/commit/08e3ca331b08ba0da503bf0e00f6f5fd1fd69a10))
* **vm:** now disables disk cache from 4tb ([#632](https://github.com/equinor/sepes-api/issues/632)) ([9047a14](https://github.com/equinor/sepes-api/commit/9047a140d4bedeb029dcc1f0a8d6ab72e9fcea1f))
* azurekeyvaultservice logger injection ([51222c9](https://github.com/equinor/sepes-api/commit/51222c92534789362ad53b9e6364f942f10c57f4))

### [0.4.14](https://github.com/equinor/sepes-api/compare/0.4.13...0.4.14) (2021-03-18)


### Bug Fixes

* **dataset:** better handling of missing resource group edge case ([#601](https://github.com/equinor/sepes-api/issues/601)) ([94fa316](https://github.com/equinor/sepes-api/commit/94fa31623a8904d6473c9394e061abeb1390ccd7))
* **DatasetUtils:** add nullcheck ([#618](https://github.com/equinor/sepes-api/issues/618)) ([1e64848](https://github.com/equinor/sepes-api/commit/1e6484821d9c17438fe41ed1b96a69cb6273002d))
* **sandbox:** better null handling and logging in vnet creation ([#610](https://github.com/equinor/sepes-api/issues/610)) ([bbc7238](https://github.com/equinor/sepes-api/commit/bbc723859b8bb19dfcb903397720ce1dd5b26699))
* **sandbox:** changed when resource retry link is created ([#600](https://github.com/equinor/sepes-api/issues/600)) ([baec250](https://github.com/equinor/sepes-api/commit/baec250c264a77894b51da3ac1d2cae228122281))
* **sandbox:** timeout when going to next phase ([#621](https://github.com/equinor/sepes-api/issues/621)) ([b4b901a](https://github.com/equinor/sepes-api/commit/b4b901abeac65c2c089f99566c338caaded61ef1))
* **study:** increase performance for GET resultsandlearnings ([#607](https://github.com/equinor/sepes-api/issues/607)) ([8bdedc5](https://github.com/equinor/sepes-api/commit/8bdedc5d6ef520124cb0e48c62046d9c25de5d10))
* **study:** prevented details request deadlock ([#611](https://github.com/equinor/sepes-api/issues/611)) ([d2c0859](https://github.com/equinor/sepes-api/commit/d2c085977f8110135995befcecd4ac50384de629))
* **worker:** added trace logging in startup ([#620](https://github.com/equinor/sepes-api/issues/620)) ([7c86c3b](https://github.com/equinor/sepes-api/commit/7c86c3bbf3402518c78589f8bca2306c74a5269e))
* standardized how azure services handles non existing resources ([#599](https://github.com/equinor/sepes-api/issues/599)) ([13d565f](https://github.com/equinor/sepes-api/commit/13d565f4c8eb204ef6b656a43d6c37253c014113))
* Validate firewall IP rules ([1866644](https://github.com/equinor/sepes-api/commit/18666447832bbb0c0d8fcd514af345a9448a8349))
* **study:** streamlined create and add logo ([#603](https://github.com/equinor/sepes-api/issues/603)) ([77efc2b](https://github.com/equinor/sepes-api/commit/77efc2bb02e7181f3cb779eefb16b386c1790e3c))
* **worker:** sandbox creation queue timeout and better in prog protection ([#613](https://github.com/equinor/sepes-api/issues/613)) ([1a0500e](https://github.com/equinor/sepes-api/commit/1a0500e06df9ba71358a0995b6ef44c8c8cabe00))

### [0.4.13](https://github.com/equinor/sepes-api/compare/0.4.12...0.4.13) (2021-03-04)


### Bug Fixes

* **dataset:** ensuring container exist when getting file list ([#597](https://github.com/equinor/sepes-api/issues/597)) ([e202ea3](https://github.com/equinor/sepes-api/commit/e202ea33c588d801ae8d05e20352fe309df0ba71))
* **dataset:** getting server ip retries 3 times and caching result ([#586](https://github.com/equinor/sepes-api/issues/586)) ([ce8fd96](https://github.com/equinor/sepes-api/commit/ce8fd961be0fc7dec5333a0ded9780821f92dd4a))
* **dataset:** new sas token endpoing for deleting files ([#592](https://github.com/equinor/sepes-api/issues/592)) ([b818d10](https://github.com/equinor/sepes-api/commit/b818d1082b1ac00015e9a2c24edb4e0f9e7ec646)), closes [#590](https://github.com/equinor/sepes-api/issues/590)
* **monitoring:** improved log messages, added standardized eventids ([#580](https://github.com/equinor/sepes-api/issues/580)) ([37cf7f4](https://github.com/equinor/sepes-api/commit/37cf7f4515e6656ea871c68fba464f03a293750a))
* **sandbox:** added null checks to resource endpoint ([607d8db](https://github.com/equinor/sepes-api/commit/607d8dba15105ae5d7125df00f69e025f6321d03))
* **sandbox:** fixed mapping error occuring in sandbox response. ([#582](https://github.com/equinor/sepes-api/issues/582)) ([e01224f](https://github.com/equinor/sepes-api/commit/e01224f23004596deef11978ab9bec36277dc224))
* **study:** remove logo if logoUrl is empty ([#589](https://github.com/equinor/sepes-api/issues/589)) ([60aa506](https://github.com/equinor/sepes-api/commit/60aa506d0c306a54b4ae14a144197644ee2d2e4c)), closes [#588](https://github.com/equinor/sepes-api/issues/588)

### [0.4.12](https://github.com/equinor/sepes-api/compare/0.4.11...0.4.12) (2021-02-24)


### Features

* **dataset:** showing restriction based on selected datasets ([#550](https://github.com/equinor/sepes-api/issues/550)) ([223db73](https://github.com/equinor/sepes-api/commit/223db733b22450309a9d20b4089c224a9c47b8fc))


### Bug Fixes

* **dataset:** increased upload sas token timeout to 30m ([64360c4](https://github.com/equinor/sepes-api/commit/64360c4a5b9b1059e92b5b08889339bb752dbc95))
* **participant:** fixed error in participant lookup where same role was added multiple times ([#567](https://github.com/equinor/sepes-api/issues/567)) ([c8d79c9](https://github.com/equinor/sepes-api/commit/c8d79c99b2970e80b17e1b544e28247b932ae911))
* **sandbox:** fixed mapping in resource response ([#568](https://github.com/equinor/sepes-api/issues/568)) ([a8673a4](https://github.com/equinor/sepes-api/commit/a8673a4937b668c41d6a591b9c8fb82210094787))
* add nullchecks for azureVmUtil and add tests ([b64cf0d](https://github.com/equinor/sepes-api/commit/b64cf0d9b2081ff02a9f402d7470b3d2c8f5904c))
* better handle TaskCancelledException and dont log as error ([#558](https://github.com/equinor/sepes-api/issues/558)) ([01c402c](https://github.com/equinor/sepes-api/commit/01c402cd233627cde4fb9d10c3163303a2bd3c66)), closes [#483](https://github.com/equinor/sepes-api/issues/483)
* New endpoint for study roles where it will return which role a u… ([#560](https://github.com/equinor/sepes-api/issues/560)) ([673283b](https://github.com/equinor/sepes-api/commit/673283b7980f6a60e0dbdbc93faae2cdfdeb1e3a))
* **dataset:** now adding correct client ip firewall rule ([ce3c464](https://github.com/equinor/sepes-api/commit/ce3c464dea5bb86093daf51cdef9986f06f2fcff))
* **sandbox:** removed duplicate check of existing name ([71a2abc](https://github.com/equinor/sepes-api/commit/71a2abc5b28ab3671ff5ffb3c00bc00884a1116f))

### [0.4.11](https://github.com/equinor/sepes-api/compare/0.4.10...0.4.11) (2021-02-19)

### [0.4.10](https://github.com/equinor/sepes-api/compare/0.4.9...0.4.10) (2021-02-19)

### [0.4.9](https://github.com/equinor/sepes-api/compare/0.4.8...0.4.9) (2021-02-19)

### [0.4.8](https://github.com/equinor/sepes-api/compare/0.4.7...0.4.8) (2021-02-18)

### [0.4.7](https://github.com/equinor/sepes-api/compare/0.4.6...0.4.7) (2021-02-18)

### [0.4.6](https://github.com/equinor/sepes-api/compare/0.4.5...0.4.6) (2021-02-10)


### Bug Fixes

* add endpoint for getting sas key ([#541](https://github.com/equinor/sepes-api/issues/541)) ([10b9103](https://github.com/equinor/sepes-api/commit/10b9103f890809dda82ee36bb35676e1609d67c7))
* **studies:** external users not  seeing all studies in list anymore ([#537](https://github.com/equinor/sepes-api/issues/537)) ([3bd15bd](https://github.com/equinor/sepes-api/commit/3bd15bd09f60173aecaf9f2b0b41ca63992f207e))

### [0.4.5](https://github.com/equinor/sepes-api/compare/0.4.4...0.4.5) (2021-02-03)

### [0.4.4](https://github.com/equinor/sepes-api/compare/0.4.3...0.4.4) (2021-02-03)


### Bug Fixes

* **dataset:** ensuring rg exist on every dataset create ([d6cd6d9](https://github.com/equinor/sepes-api/commit/d6cd6d9258c5890656b8b74912c00fb2f05f3c08))
* **dataset:** now checking study specific access when reading dataset ([#533](https://github.com/equinor/sepes-api/issues/533)) ([06f40fc](https://github.com/equinor/sepes-api/commit/06f40fcea5fe8aa11e27f269e57159420f988d67)), closes [#532](https://github.com/equinor/sepes-api/issues/532)
* **dataset:** set azure role assignments for storage account and resource group ([#513](https://github.com/equinor/sepes-api/issues/513)) ([0949136](https://github.com/equinor/sepes-api/commit/09491361e13b9646355cefda0b4d3bab163cedc0))
* **participants:** issue where sepes was dependent on Azure to return… ([#502](https://github.com/equinor/sepes-api/issues/502)) ([f34f980](https://github.com/equinor/sepes-api/commit/f34f980bcd635de0832ab4f2b0ef42dfda7e7448)), closes [#499](https://github.com/equinor/sepes-api/issues/499)
* **rbac:** now allowing b2b/external user access ([#531](https://github.com/equinor/sepes-api/issues/531)) ([ac61458](https://github.com/equinor/sepes-api/commit/ac61458549a557960b9e7f83c08391adb8b6efb5))
* **sandbox:** new endpoint for getting cost analysis ([1f8d837](https://github.com/equinor/sepes-api/commit/1f8d8374dd41b3f4b76c4592ede730a760c2d74a))
* **study:** delete sometimes failed when no resource group created ([#525](https://github.com/equinor/sepes-api/issues/525)) ([558c483](https://github.com/equinor/sepes-api/commit/558c483028108426613a45686cbd064a8e8dd5e9))
* **study:** now returning lighter response for study details endpoint ([15c4f02](https://github.com/equinor/sepes-api/commit/15c4f026938f0f563e7b70164d51abd283dc5593))
* **vm:** issue with password validation: Updated test ([fc0577f](https://github.com/equinor/sepes-api/commit/fc0577f31478599862dc02b6ef4916e0cd63767f)), closes [#504](https://github.com/equinor/sepes-api/issues/504) [#503](https://github.com/equinor/sepes-api/issues/503)
* **vm:** potential fix for failed creation in other users sandbox ([ade6540](https://github.com/equinor/sepes-api/commit/ade6540d82081530a53d4772ff15b476e2fb625a))
* **vm:** username validation now takes what os is picked into consideration ([2ff065d](https://github.com/equinor/sepes-api/commit/2ff065d6f6caa49dc4ac5c5c14dd60d0af5a3c46)), closes [#521](https://github.com/equinor/sepes-api/issues/521)
* **worker:** add error to resource ([#516](https://github.com/equinor/sepes-api/issues/516)) ([80f98da](https://github.com/equinor/sepes-api/commit/80f98da82da898c2f4b541d169a955143530ea9c))
* issue where space was not allowed in study, sandbox and vm name ([f77ccb8](https://github.com/equinor/sepes-api/commit/f77ccb85e37d041b53ca01aed880dfab5981f3e7))
* Require names of studies, vms and sandboxes to have 3 or more ch… ([#509](https://github.com/equinor/sepes-api/issues/509)) ([bd169eb](https://github.com/equinor/sepes-api/commit/bd169eb95e37aba0f428ddc9c7c2c6857d6dc498)), closes [#495](https://github.com/equinor/sepes-api/issues/495)

### [0.4.3](https://github.com/equinor/sepes-api/compare/0.4.2...0.4.3) (2021-01-21)


### Features

* **roles:** ensure study roles are set in Azure  ([#484](https://github.com/equinor/sepes-api/issues/484)) ([7c74fcd](https://github.com/equinor/sepes-api/commit/7c74fcde877ca2d91b46a22fe9e06164d5cbe1e1))
* **sandbox:** new endpoint that returns all available datasets, and if they are added to sandbox ([e8dbfee](https://github.com/equinor/sepes-api/commit/e8dbfeec888a04805443e1266d35d90f686f1266))


### Bug Fixes

* **dataset:** added permissions to study specific dataset response (c… ([#471](https://github.com/equinor/sepes-api/issues/471)) ([1fb5b4e](https://github.com/equinor/sepes-api/commit/1fb5b4e42f5fc9faf451f0bc69b254a88687f85d))
* **dataset:** now passing correct arguments to role assignment service ([#492](https://github.com/equinor/sepes-api/issues/492)) ([e27ab4c](https://github.com/equinor/sepes-api/commit/e27ab4c13d7e6e49d32859a3856cdc407820c974))
* **logging:** dont log all provisioning exceptions as exceptions to App Insights ([#498](https://github.com/equinor/sepes-api/issues/498)) ([037e4c7](https://github.com/equinor/sepes-api/commit/037e4c7fd530adeaf82c7631efeb36f0139b886f))
* **sandbox:** add/remove dataset. Improved validation and messages. Now returning HTTP 204 on succes ([d6f534c](https://github.com/equinor/sepes-api/commit/d6f534cbce43ff1794b81605c3e87cf343abf8dc))
* **sandbox:** Include disk price to the estimated cost of an vm ([#469](https://github.com/equinor/sepes-api/issues/469)) ([cd06148](https://github.com/equinor/sepes-api/commit/cd06148be05760c7a6706c4d9d2dde1252d7aaf3))
* **vm:** add endpoint for validating a username. Some names and perio… ([#479](https://github.com/equinor/sepes-api/issues/479)) ([6394c24](https://github.com/equinor/sepes-api/commit/6394c24471930f99d8c4ca278f017a24f3f30962)), closes [#478](https://github.com/equinor/sepes-api/issues/478)
* **vm:** improved selection of price for all SKUs. Avoiding "low priority" and "spot" if possible ([acec3d4](https://github.com/equinor/sepes-api/commit/acec3d4099e7ed73ce2136c0a722da5cc0957df6))
* **vm:** new endpoint for getting price of an vm without calling azure ([43aa823](https://github.com/equinor/sepes-api/commit/43aa8232ce84385f9b3efeab3d84061dd59e2c87))
* **vm:** now rolling back, if creation fails in the create response ([#490](https://github.com/equinor/sepes-api/issues/490)) ([91b87f9](https://github.com/equinor/sepes-api/commit/91b87f9bf33d042a1fa5d29673eb2a5f73e447b8))
* **vm:** provisioning can now resume if last attempt failed ([#491](https://github.com/equinor/sepes-api/issues/491)) ([084e030](https://github.com/equinor/sepes-api/commit/084e0300c02e320478ddde3781f98b48d151b029))
* **vm:** validate password and throw error if it does not meet requir… ([#476](https://github.com/equinor/sepes-api/issues/476)) ([2b1cd7b](https://github.com/equinor/sepes-api/commit/2b1cd7be5738676b5df138c04832a0bf02495192)), closes [#337](https://github.com/equinor/sepes-api/issues/337)
* **vm:** vm name validation can now handle special characters. For instance question mark ([#482](https://github.com/equinor/sepes-api/issues/482)) ([050f53a](https://github.com/equinor/sepes-api/commit/050f53af5cc20ac4f812464f4484edefb28da141)), closes [#481](https://github.com/equinor/sepes-api/issues/481)
* **vm:** vm not getting stuck in updating if previous attempt failed ([#470](https://github.com/equinor/sepes-api/issues/470)) ([b9e5685](https://github.com/equinor/sepes-api/commit/b9e568540e36e94ac8e0326a29bba2d5717cac16))
* ensured relevant tags are being set for all resources ([309eba4](https://github.com/equinor/sepes-api/commit/309eba4c65d624b5f44156e268bd026242a11c6f))

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
