# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## 1.0.0 (2021-08-24)


### ⚠ BREAKING CHANGES

* url for deleting study specific dataset is now on api/studies/datasets/studyspecific/{datasetId}
* Removed support for CRUD operations for pre-approved datasets

* test: DatasetFileService
* Study details response does not contain resultsAndLearnings property

* fix: now able to get study even though logo functionality is failing

previously, all methhods for study was failing if something went wrong with logo

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

* feat(dataset): implemented file upload for study specific datasets

Also updated relevant unit tests
* three endpoints described above are replaced by endpoints not requiring the
studies/studyId part
* Method HandleAddParticipantAsync now returns StudyParticipant entry instead of
Study entry
* **deployazureresources:** Now requires admin for all endpoints
* **deployazureresources:** New DTO/form for creating sandbox. Only name, region and template needed. Template
is still not supported, so the field
* **sandboxservices:** Using api to create Sandbox now requires valid azure credentials to work properly,
as it now does stuff in azure

https://dev.azure.com/equinor-core-dev/Sepes/_boards/board/t/DevOps/Stories/?workitem=1856
* **azureblobstorageservice:** need To have azure container named studyLogos
* **dataset, studycontroller, studyservice, automapperconfig:** Migration, adds a field in datab
* **studycontroller:** Swagger is broken. Cant upload image there anymore
* **studyservice and logo:** Migration: Remove StudyLogos Table, also changed fields in Study. Remove LogoId and
added LogoUrl.
* **datamodel:** Data model for study changed

### Features

* **ad azure user search:** added endpoint to get users by searchstring ([779919b](https://github.com/equinor/sepes-api/commit/779919b8892308c489bd07809eeaa78f91cd98ce))
* **add swagger:** adding swagger. CHanged launchSettings so VS starts app on Swagger page ([1c75d56](https://github.com/equinor/sepes-api/commit/1c75d5606c288749295a9ec4f445bf069d4c668f))
* add wbs validation from external api ([#677](https://github.com/equinor/sepes-api/issues/677)) ([f607cf9](https://github.com/equinor/sepes-api/commit/f607cf9e976888ec6e57297ff062181a57d3ba6a))
* **added tag functionality:** added tag functionality and pulling together most relevant tags ([91c0377](https://github.com/equinor/sepes-api/commit/91c03776f50a5ad30fc41fd292038f38712713ab))
* **addshellfornewstudyendpoints:** added todo comments and enpty implementations ([90e5d00](https://github.com/equinor/sepes-api/commit/90e5d006eb832a4782a4fde71c8104167d6ad889))
* **adduserservice:** added a user service instead of the claims principal service ([0a2802f](https://github.com/equinor/sepes-api/commit/0a2802f31ee05ffd07acec4bfc4919ec2b6fdc75))
* **all azure services and interfaces:** added Exists(), made stgAccount check name availability ([9998545](https://github.com/equinor/sepes-api/commit/99985457f2dafb158e1ae0665b98407e5d62905e))
* **auth:** Progress. Replaced add auth and token validation. Had to add values to appsettings. will probably have to clean this out ([a9dd209](https://github.com/equinor/sepes-api/commit/a9dd20970fce6adc3ae3f5234aa71f23845ad24c))
* **azure search users:** fixed up the merge from develop. merge gone bad ([a295063](https://github.com/equinor/sepes-api/commit/a29506343f40d7d90cf5d65cdb4050f2225956c0))
* **azure search:** radixconfig, added AzureAd__ prefix ([d09111f](https://github.com/equinor/sepes-api/commit/d09111f4e627956963a3d7fa8414a8db61792dfc))
* **azure worker for sandbox resources:** some initial refactoring of services ([8c8db41](https://github.com/equinor/sepes-api/commit/8c8db41f04c853127649c0e1298a64b8fc1e147b))
* **azure worker:** endpoint for resources. Now updates provsisioning state after creation ([877ea2c](https://github.com/equinor/sepes-api/commit/877ea2c25792b94a2f5c7ccea796566e0d6736d4))
* **azure worker:** implemented create for all basic sandbox resource ([952c1e5](https://github.com/equinor/sepes-api/commit/952c1e5709fef2bba35b1f6c03347d0732a79d4c))
* **azure worker:** now also binds nsg to subnet ([8edd385](https://github.com/equinor/sepes-api/commit/8edd3854d30d6daa4388302131517d051c95e8af))
* **azure worker:** redefined how work is pulled. Cleaned DTOs. Redefined queue ([2589b2c](https://github.com/equinor/sepes-api/commit/2589b2c6e538ce6fd858f219a53895f0442f5502))
* **azure worker:** redefined how work is pulled. Cleaned DTOs. Redefined queue ([1f8b82a](https://github.com/equinor/sepes-api/commit/1f8b82ab8146f7299e6f59e846efa378ed635aed))
* **azure worker:** refactor and make it testable. Mocking azure resource group creation ([a258955](https://github.com/equinor/sepes-api/commit/a258955ee3edcf9355621cc2f32d324dc5c850f8))
* **azuread:** Refactored azure ad authentication. Now uses environment variables ([cf0860e](https://github.com/equinor/sepes-api/commit/cf0860e04530d72f8bd7b517c7b3a31ac3c60535))
* **azurebastionservice:** now creates a Bastion succesfully ([fbf4c12](https://github.com/equinor/sepes-api/commit/fbf4c1202a69a8b221991ffd6e67c6a1ee6f6df9))
* **azurequeueinterface:** updated types of AzureQueueInterface ([38ef6c1](https://github.com/equinor/sepes-api/commit/38ef6c1b3095464841ca51de317d1d626861ce0d))
* **azurequeueservice + tests:** made test class for QueueService, and improved functionality ([753006b](https://github.com/equinor/sepes-api/commit/753006b8e98da12daea733f9bccf31279c128454))
* **azurequeueservice, appsettings.json:** started writing AzureQueueService, Added Connectionstring ([5a7e67c](https://github.com/equinor/sepes-api/commit/5a7e67c8e6b8f339e005607d9749f5be1f2c666b))
* **azurequeueservice, iazurequeueservice:** azureQueueService is now implemented ([65d0380](https://github.com/equinor/sepes-api/commit/65d0380effc20a155ddae64a9736d3408e975b47))
* **azureresourcemonitoringservice, ihasexists:** created initial method for checking orphan resourc ([71206f0](https://github.com/equinor/sepes-api/commit/71206f04028dd5061faebd3f4a6b641d28b7c4a1))
* **azureresourcemonitoringservice:** added Tag check/update to resource monitor ([051baeb](https://github.com/equinor/sepes-api/commit/051baeb72d95306092850516e8f0afc20d11c828))
* **azureresourcenameutil:** added method for creating name for storagAccounts ([1b71c95](https://github.com/equinor/sepes-api/commit/1b71c95f26c0929a94e411461195f13359335fdc))
* **azureresourcenameutil:** naming of ResourceGroups now ensure uniqueness ([5f3cfba](https://github.com/equinor/sepes-api/commit/5f3cfba66b516eced1515537359ac3e3f8bde79f))
* **azureresourcenameutils:** add methods for cleaning sandboxName. Also made sure stgAcc are unique ([da62255](https://github.com/equinor/sepes-api/commit/da62255b145414fa947b02cf02f433ecae591b85))
* **azureservice, azureresourcegroupservice:** implemented NukeUnitTestSandboxes() ([a7e6a30](https://github.com/equinor/sepes-api/commit/a7e6a3036dcea0a5bd85a0d63a905be8a385160a))
* **azureservicetest:** progress on azureservice test, but not working ([c6779c7](https://github.com/equinor/sepes-api/commit/c6779c7e0abaf511e2afdc7761d204b25e00e67a))
* **azurestorageaccountservice:** beginning of a storageAccountService ([2701fd4](https://github.com/equinor/sepes-api/commit/2701fd42e668dddf57a269f4fdf455a6aea29336))
* **azurevmservice:** can now create a Windows VM ([54cfba6](https://github.com/equinor/sepes-api/commit/54cfba641ef692b7ab6430be60015b04226d0c21))
* **azurevmservice:** initial commit of work on AzureVService ([3f552e1](https://github.com/equinor/sepes-api/commit/3f552e1390ca78ea83ff42512533ae9b926dd196))
* **azurevmutils, azurevmservice, azureservice:** can now create VMs, with Boot diagnostics ([33a0b58](https://github.com/equinor/sepes-api/commit/33a0b58894581e633b316baf853a42d3fe8d68a7))
* **azureworker:** now scheduling work into queue and picking up on the other side ([51730bc](https://github.com/equinor/sepes-api/commit/51730bc21cbb388c29311ff094305abdae7ed5d1))
* **changed default log level to info:** changed log level ([60f29ec](https://github.com/equinor/sepes-api/commit/60f29ec866d3e2520c07da47b3ad0412405cf5b3))
* **changed lifecycle to user service to scoped:** userService and IPrincipalService is now Scoped ([f8c958f](https://github.com/equinor/sepes-api/commit/f8c958f85f5087c4bb024fa9c62810b33374a70d))
* changing auth to api auth ([9196ff2](https://github.com/equinor/sepes-api/commit/9196ff214914ab59d3767ac6cf6b893b86e13313))
* connected Study with Sandbox and DataSet ([c605056](https://github.com/equinor/sepes-api/commit/c6050560c4d25552d003e2c2b45b5e1bacae9868))
* **createsandboxbetterresult:** added missing GetSandbox endpoint and service method ([b182af6](https://github.com/equinor/sepes-api/commit/b182af6e39d352535af0bfc2c7bf3657872dac9b))
* **database:** Enabled EF Core migrations. Now using separate connection string for migrations vs other DB IO ops. ([55daefb](https://github.com/equinor/sepes-api/commit/55daefb2c2b55ae712b982edfbfa8a25ff8bf959))
* **datamodel:** starting work on data model for study. Got automapper to work for study ([884689a](https://github.com/equinor/sepes-api/commit/884689ade891648277e51076630ccc2e3b0efac8))
* **dataset-dto:** added studyId field to check for studysepecific datas ([bc41a56](https://github.com/equinor/sepes-api/commit/bc41a563f20421f511931af0b2767fce8bfce6ac))
* **dataset, datasetservice:** added StorageAccountName, and made some fields required ([e359365](https://github.com/equinor/sepes-api/commit/e35936556810cf23c8a4433c0947f5268acd4a3c))
* **dataset, studycontroller, studyservice, automapperconfig:** adding StudySpecificDatasets ([a272df4](https://github.com/equinor/sepes-api/commit/a272df41ffd0999d2a2bd8f0bed933080df7b2d4))
* **datasetcontroller, datasetservice:** made get dataset endpoint return more details ([c65505e](https://github.com/equinor/sepes-api/commit/c65505e8bb6760687c627e32757de5b64e0d1b9b))
* **datasetcontroller, studycontroller and servives:** added put method for datasets ([cebc364](https://github.com/equinor/sepes-api/commit/cebc364295f4d66d1d7f7bcb8f3684fd1a08b9d6))
* **dataset:** deny connections and make available to sandbox ([#454](https://github.com/equinor/sepes-api/issues/454)) ([844c710](https://github.com/equinor/sepes-api/commit/844c7102ac577a8135dd4a1fe49f8460ed36575a)), closes [#458](https://github.com/equinor/sepes-api/issues/458)
* **datasetservicetests, studycontroller, study:** added field resultsandlearnings in Study ([c496cb3](https://github.com/equinor/sepes-api/commit/c496cb3e8cb31612d878e113030f56436107e6ea))
* **dataset:** showing restriction based on selected datasets ([#550](https://github.com/equinor/sepes-api/issues/550)) ([223db73](https://github.com/equinor/sepes-api/commit/223db733b22450309a9d20b4089c224a9c47b8fc))
* **dblogging, sandboxworkerservice:** provides better serviceOperationDescriptions. added in db ([e513e45](https://github.com/equinor/sepes-api/commit/e513e45dbda1459a8d78d49695d39af6087c0104))
* **delete sandbox:** implemented delete sandbox. DB entires are kept, but marked as deleted ([5bde249](https://github.com/equinor/sepes-api/commit/5bde24900b05417511ba9a5ef15ca64b39193526))
* **deplazureresrefactorandtest:** fixed unit test for azure res. Some refactor. Fixed res name util ([a350da9](https://github.com/equinor/sepes-api/commit/a350da96bede9c1ea1af12132ce5dcf2b53e3a3e))
* **deploy azure resources monitoring:** implemented methods for active resources and prov state ([e80085a](https://github.com/equinor/sepes-api/commit/e80085acc4d5d135b8783e8a03c222d1ae3bc23c))
* **deployazrueresources:** progress, now creating PIP, but still err on bastion ([eb29b8b](https://github.com/equinor/sepes-api/commit/eb29b8b47ed0401cad5816a54350f989b45816ad))
* **deployazurefeatures:** renamed from CloudResource to SandboxResource ([01e72b8](https://github.com/equinor/sepes-api/commit/01e72b8f124df5171711fa362ea77f8c3fa68736))
* **deployazureres:** added technical contact for sandbox. Connected cloudres with sandbox ([7123566](https://github.com/equinor/sepes-api/commit/71235668756b5bac89fedc2bfd2c0fbeed58367d))
* **deployazureresource:** refactor, cleaning and some improvements ([a4c19e3](https://github.com/equinor/sepes-api/commit/a4c19e3ce0e4eaaa84c6a0978f0f551de3781036))
* **deployazureresources and rbac:** started work on RBAC for studies. Also dvided StudyController ([4a1980e](https://github.com/equinor/sepes-api/commit/4a1980eac5c31d55768eb66a55c41ffaf5d38eb2))
* **deployazureresources monitoring:** started work on monitoring of az resources ([facb4cf](https://github.com/equinor/sepes-api/commit/facb4cfd479e0ab745b68feae7527bd33b672eb6))
* **deployazureresources:** added variable table and service ([9e083a0](https://github.com/equinor/sepes-api/commit/9e083a0f0eefcce606b61e88b2554ff03cd49e92))
* **deployazureresources:** changed way creating of azure resources is disabled ([f2ab85f](https://github.com/equinor/sepes-api/commit/f2ab85fa14741567b58c824a89e1b9ba6ec89b6b))
* **deployazureresources:** enforced admin role requirement. Disabled create azure resources ([780fc65](https://github.com/equinor/sepes-api/commit/780fc65a2bef3994a02aa0274a23c26e132b2967))
* **deployazureresources:** implemented SandboxResourceOperationService with basic methods ([18863e7](https://github.com/equinor/sepes-api/commit/18863e707857005cf20a9fa9e6c636d93a71d8c2))
* **deployazureresources:** progress and cleaning ([fa06ee6](https://github.com/equinor/sepes-api/commit/fa06ee6cfdfd581d35c32a1c9b5eb05773873858))
* **deployazureresources:** progress and cleaning ([473f90e](https://github.com/equinor/sepes-api/commit/473f90e98c773dd204fdaf310060f213e6157965))
* **deployazureresources:** progress, now workong in bastion ([ae874fd](https://github.com/equinor/sepes-api/commit/ae874fd15464f948f270595823432afde2fe7b95))
* **deployazureresources:** some clening and refactoring. Also impl delete for all types ([6b103bb](https://github.com/equinor/sepes-api/commit/6b103bbe4ffe4636704a99192860cc7554a23d4e))
* **deployazureresources:** startet work on the services that will deploy resources to azure ([b304f9e](https://github.com/equinor/sepes-api/commit/b304f9e868e5b5a54ffb7cae24dc1ba299b05b36))
* **deployazureresources:** starting work on deploying sandboxes to azure(Network, VMs etc) ([23bd18a](https://github.com/equinor/sepes-api/commit/23bd18a1b2a425280f963e292aa513d4ae8fd99c))
* **deployazureresources:** still stuck on bastion host ([b3cb2cd](https://github.com/equinor/sepes-api/commit/b3cb2cdc469d31e26cd7ac979d837b1b70b69ff7))
* **deployazureresource:** started work on a unit test that can generate azure resources ([de9912d](https://github.com/equinor/sepes-api/commit/de9912d9bc0d1b17fb7a4d4f240206cee1daab0e))
* **developmentenv:** Cleaned out old auth code. Added appsettings.Development to gitignore ([7738487](https://github.com/equinor/sepes-api/commit/7738487340960c5c7dfd968fef8a343b924870be))
* **dockerize:** Renamed Dockerfile inside RestApi folder. It's not used as of now. Added docker-compose to gitignore and added a sample docker-compose file to doc folder ([fc5b03d](https://github.com/equinor/sepes-api/commit/fc5b03dbc70b65410f7d91410be2033e00a651d9))
* enabled norwaywest, europenorth and europewest ([#631](https://github.com/equinor/sepes-api/issues/631)) ([10eab76](https://github.com/equinor/sepes-api/commit/10eab76dcd99791061e6bf9cc5b544bfba302580))
* **errorhandlingandresponse:** starting implementing error handling and response ([795cb89](https://github.com/equinor/sepes-api/commit/795cb890ea8c2c3d53f87c05ca1aa92a1c84877c))
* **fix vendor and wbs in studyservice:** fixed Put method of study ([ff2001d](https://github.com/equinor/sepes-api/commit/ff2001d67bd5072f252b07669cb90fc2c28cc42a))
* **get studies rbac improvement:** now in accordance to the permission matrix ([80cf2db](https://github.com/equinor/sepes-api/commit/80cf2dbe51baf85379b9d7906b7d71c914411470))
* **getdatasetforstudybyid:** added endpoint for getting a specific dataset for specifica study ([d35c274](https://github.com/equinor/sepes-api/commit/d35c274d048a4dc2239518ffdd9a9e61750da555))
* **graph search:** azure config now in AzureAd section. Added mapping for graph response ([f8f1fb0](https://github.com/equinor/sepes-api/commit/f8f1fb07d8961d1989fb55d9093308809db1d22c))
* **improve sandbox response and dto:** cleaned up returned fields. Also including resources ([ab3c528](https://github.com/equinor/sepes-api/commit/ab3c5286f4182763e942ce0317cb76fa842ecf0a))
* **logosastoken:** finished logo sas token ([01e75fc](https://github.com/equinor/sepes-api/commit/01e75fc6cc3403805f4d0c0d9fc8b9650a2ec4eb))
* **models, and dtos:** added Required Fields in DB ([7c32308](https://github.com/equinor/sepes-api/commit/7c32308d7171f99eee6858e8916bcc19471cebea))
* **more rbac:** better connection between Ad and User table. Adding role on study creation ([b827072](https://github.com/equinor/sepes-api/commit/b827072b45c3e743b7c162839910884b23c4b4fb))
* **participants:** new endpoint to add users outside azure ad ([3f5d4dd](https://github.com/equinor/sepes-api/commit/3f5d4ddcabbce6d4bff47544b1721317dde8c026))
* **radix:** Renamed env variables to SEPES naming convention ([375e4aa](https://github.com/equinor/sepes-api/commit/375e4aac142e89eccd98a6a3b36b1abe6ce52c9f))
* **radix:** Updated radix config, removed empty build section ([f603643](https://github.com/equinor/sepes-api/commit/f6036434b1a106a4a2e0e36759353689b48b6a87))
* **removed norway west region:** resources should not be placed there anyway ([23490c3](https://github.com/equinor/sepes-api/commit/23490c376179b6239093f3d4e0bc30fe8af04eb5))
* **renamecloudresourcetosandboxresource:** renamed models, services and dtos ([fd74460](https://github.com/equinor/sepes-api/commit/fd74460d08c422a751e497f1f82186e32cf80002))
* **roles:** ensure study roles are set in Azure  ([#484](https://github.com/equinor/sepes-api/issues/484)) ([7c74fcd](https://github.com/equinor/sepes-api/commit/7c74fcde877ca2d91b46a22fe9e06164d5cbe1e1))
* **sandbox, study, controller and service:** "Delete Sandbox from Study" endpoint is implemented ([c0e368a](https://github.com/equinor/sepes-api/commit/c0e368af8be601d9983a2d2042d2ab53241cf013))
* **sandbox:** add link to cost analysis ([#412](https://github.com/equinor/sepes-api/issues/412)) ([2c42d3d](https://github.com/equinor/sepes-api/commit/2c42d3d399ce797427ffd97d949094aaade1be1e)), closes [#342](https://github.com/equinor/sepes-api/issues/342)
* **sandbox:** new endpoint that returns all available datasets, and if they are added to sandbox ([e8dbfee](https://github.com/equinor/sepes-api/commit/e8dbfeec888a04805443e1266d35d90f686f1266))
* **sandboxresourceservice:** can now get "deleted" resources from db + refactor: use expressions ([d921719](https://github.com/equinor/sepes-api/commit/d92171936921229608ba7a34d6282ccea4ab10c9))
* **sandboxservices:** creating sandbox will now do stuff in Azure ([b054dc8](https://github.com/equinor/sepes-api/commit/b054dc8d2e864ec0e30f860f88fb110db4acf2f6))
* **sandboxworkerservice => dowork():** method is no longer empty, however it is not done ([4f1a54e](https://github.com/equinor/sepes-api/commit/4f1a54eb591752c7c282935ef4b9c668b71d930e))
* **sastokenforlogos:** now generating sas token for logos ([8a02862](https://github.com/equinor/sepes-api/commit/8a02862b9c350778fded6eab6c2fd31e50aa41ba))
* **services that handle db:** update functions now sets Updated-field to UTC DateTime object ([6999b36](https://github.com/equinor/sepes-api/commit/6999b36f260280c3bfbc0fb8b170061b3313c040))
* **set study creator as owner:** had some dead references after last rename ([d025f3b](https://github.com/equinor/sepes-api/commit/d025f3b674666e0947359761fa71d4efda540194))
* **set study creator as owner:** when a user creates a study, he is added as owner for that study ([91a8949](https://github.com/equinor/sepes-api/commit/91a8949bd41da47830ae0f1e2cdec71d360a5a18))
* **study and sandbox:** can now add Sandbox to Study ([687a7dc](https://github.com/equinor/sepes-api/commit/687a7dc6e00efa95852f62de09c04c46718ae36f))
* **study, datasetstudy, dataset, dtos, startup, automapper:** endpoint for Add Dataset to Study ([aced067](https://github.com/equinor/sepes-api/commit/aced067d1c5ece5bfcfb7a4669fc35b92d8bfd33))
* **studyanddatasetmodel:** added DatasetController ([62c7be3](https://github.com/equinor/sepes-api/commit/62c7be323fdedfdcb7d2f4c50c4169f0b91e9a34))
* **studycontroller, studyservice:** implemented studyspecificdatasets, also handled merge from dev ([026dfb9](https://github.com/equinor/sepes-api/commit/026dfb92b84ca470ec7a6653dcbbd0cd2bc09320))
* **studylogosasgeneration:** more work on generating sas tokens. This might be it ([5462fe0](https://github.com/equinor/sepes-api/commit/5462fe04a0b1075b13795cea18bf67613a5fec6f))
* **studymodel:** added dataset and sandbox to study dto ([e222c5f](https://github.com/equinor/sepes-api/commit/e222c5f5eb08e5add96b5e6bde36055ccf5fa8ed))
* **studymodel:** added vendor and restricted ([0a0df4b](https://github.com/equinor/sepes-api/commit/0a0df4bcbaa3eb62978b7ac5c74a0c819982c250))
* **studymodelandstudyservice:** improved Study Model. Added initial tests for StudyService ([8808386](https://github.com/equinor/sepes-api/commit/88083865142f5ecc48eecae4e7722b38d8591f89))
* **studymodelvalidation:** added generic validation in a BaseService, only in use for Study ([41e999b](https://github.com/equinor/sepes-api/commit/41e999baafd35bd9980ff88395902a2ec6a9b9ad))
* **studyparticipant:** aDded model and endpoints for StudyParticipant ([f697c92](https://github.com/equinor/sepes-api/commit/f697c929daca0ecfdd5ed917ce8b02548d176a2a))
* **studyparticipant:** added model, service and endpoints for StudyParticipant ([b784692](https://github.com/equinor/sepes-api/commit/b784692d282c1d8c4be456d38fc840119bf61a79))
* **studyparticipant:** started work on study participant ([8f021d4](https://github.com/equinor/sepes-api/commit/8f021d40c2ca9ad531077514fe77b2ef5dca85fb))
* **studypermissions:** starting work on permissions for studies ([e922a3a](https://github.com/equinor/sepes-api/commit/e922a3a009f8be79a5de3e348811735fe7a8438b))
* **studyservice and logo:** can now add and get StudyLogos ([9920d21](https://github.com/equinor/sepes-api/commit/9920d210869c4737e4a271470edfdef57dd3b577))
* **studyservice, dataset:** more attributes in Dataset-model. Also better deletion ([6879a51](https://github.com/equinor/sepes-api/commit/6879a5121496bde82e0dc35f253382070ee3c457))
* **studyservice, studycontroller, azureblobstorageservice:** add and get Logo from study ([748e5df](https://github.com/equinor/sepes-api/commit/748e5df6257ddd5b6813789726e6a8a212e9d976))
* **studyservice, studycontroller:** added "Delete DatasetFromStudy"-Endpoint ([3ff529a](https://github.com/equinor/sepes-api/commit/3ff529a4d7004f9e66594e0b7268934cb1585c24))
* **studyservice.cs, studyspecificdatasetdto.cs, istudysrevice.cs, studycontroller.cs.:** buildable ([f321820](https://github.com/equinor/sepes-api/commit/f321820334e3a25d8c35359baa1694f0c8a111e0))
* **studyservice:** added "Delete Study"-endpoint ([244b84d](https://github.com/equinor/sepes-api/commit/244b84d45111e65a34b9e2d554d7326e190afb9a))
* **studyservice:** added Get Sandboxes and Get sandboxesById ([992a6b0](https://github.com/equinor/sepes-api/commit/992a6b0e914520cf38c256cc51acf14a717b516b))
* **studyservice:** removed unused endpoint ([dce52eb](https://github.com/equinor/sepes-api/commit/dce52ebc0275fc356390ca0c738b6acd592ec955))
* **studyservice:** wBS code is now required in study for sandbox creation ([5cf5e00](https://github.com/equinor/sepes-api/commit/5cf5e00f205231b5c11c29ca975bcab8801616ac))
* **study:** validate wbs on save ([#680](https://github.com/equinor/sepes-api/issues/680)) ([adf647e](https://github.com/equinor/sepes-api/commit/adf647ea9b13befab06d835594b496f8b68c0a33)), closes [#681](https://github.com/equinor/sepes-api/issues/681)
* **testazureresources:** improved DTOs for creation ([a699890](https://github.com/equinor/sepes-api/commit/a69989098f6abf188b87ed7cb17b93c34e21e0a6))
* **turned creation of azure resources back on:** also added lookup endpoints for region and templat ([6907479](https://github.com/equinor/sepes-api/commit/69074795557f534e6b568b3854da6710122a839c))
* **vm:** new endpoint for external link to virtual machine ([#422](https://github.com/equinor/sepes-api/issues/422)) ([1ae8097](https://github.com/equinor/sepes-api/commit/1ae8097dbf99200087502242a4192e3a8354826d))


### Bug Fixes

* (SandboxResourceController): methods missing "/api/" prefix. ([#436](https://github.com/equinor/sepes-api/issues/436)) ([19d3ff9](https://github.com/equinor/sepes-api/commit/19d3ff927823eea97ba24b80a30552c87a351e2c))
* add endpoint for getting sas key ([#541](https://github.com/equinor/sepes-api/issues/541)) ([10b9103](https://github.com/equinor/sepes-api/commit/10b9103f890809dda82ee36bb35676e1609d67c7))
* **add new partcipant:** changed email to emailaddress ([25661d6](https://github.com/equinor/sepes-api/commit/25661d6ef45268d05f9abe5282a746d39be3f401))
* add nullchecks for azureVmUtil and add tests ([b64cf0d](https://github.com/equinor/sepes-api/commit/b64cf0d9b2081ff02a9f402d7470b3d2c8f5904c))
* **api:** enable cors for only configured domains, used to be open for all ([#652](https://github.com/equinor/sepes-api/issues/652)) ([06bd5ea](https://github.com/equinor/sepes-api/commit/06bd5ea0031c34413f268fccb33e90b5e0f00562))
* **appi:** Fixed typo in app insight instrumentation key config ([8235d16](https://github.com/equinor/sepes-api/commit/8235d16870ee07e31fa359415b158df76c78d86c))
* **auth:** convert to PKCE auth flow ([#656](https://github.com/equinor/sepes-api/issues/656)) ([8cd0b19](https://github.com/equinor/sepes-api/commit/8cd0b1959e5c64f1e780ede969d5e13049c0e62d))
* **auth:** fixed login redirect issue. Not requiring User.Read scope on signin anymore ([1f74345](https://github.com/equinor/sepes-api/commit/1f74345f87e49744738425dd4c63be8e88121f36))
* **azure services and tests:** fixed tests and dependency injection so that AzureTest runs ([1c279af](https://github.com/equinor/sepes-api/commit/1c279af98249a01d14696077d53593352e31ab44))
* **azureblobstorageservice:** changed Azure container to studylogos ([f95e047](https://github.com/equinor/sepes-api/commit/f95e0476627f19a85410ce3861fb7422063415a2))
* **azureblobstorageservice:** using enum to validate filetypes ([d160c89](https://github.com/equinor/sepes-api/commit/d160c893ac214c3d17eb9ed80e620d3b888e19e1))
* azurekeyvaultservice logger injection ([51222c9](https://github.com/equinor/sepes-api/commit/51222c92534789362ad53b9e6364f942f10c57f4))
* **azurequeueservicetest:** was not building, commented out ([#370](https://github.com/equinor/sepes-api/issues/370)) ([10343ef](https://github.com/equinor/sepes-api/commit/10343ef9ad8237ed8fa57b167bcbca2653ada9ba))
* **azureresourcemonitoringservice:** made tag checking/updating safer ([ce78dea](https://github.com/equinor/sepes-api/commit/ce78deaf4017774417dbfd53c9dabb71b35f026b))
* **azureservice:** minor fix. Commenting ([bd352c4](https://github.com/equinor/sepes-api/commit/bd352c437aacecde787cedd17fa4b1e11c00c9cf))
* **azureservicetest:** changed region from NorwayWest to NorwayEast ([10d074d](https://github.com/equinor/sepes-api/commit/10d074da04ea8777a09dcfdf783486ee260de083))
* **basicservicecollectionfactory, studyservicetests:** fixed Dependency Injection in tests ([4e4ac00](https://github.com/equinor/sepes-api/commit/4e4ac007a9d08bb9bb60ab0a0cd1657592e9801f))
* better handle TaskCancelledException and dont log as error ([#558](https://github.com/equinor/sepes-api/issues/558)) ([01c402c](https://github.com/equinor/sepes-api/commit/01c402cd233627cde4fb9d10c3163303a2bd3c66)), closes [#483](https://github.com/equinor/sepes-api/issues/483)
* **build error in test project:** fixed error usings ([a8e1a39](https://github.com/equinor/sepes-api/commit/a8e1a39c4a06ae3fa9e828c71084faf90d3be024))
* **changelog:** contains all changes from early morning, does not look nice ([#394](https://github.com/equinor/sepes-api/issues/394)) ([ea8d1df](https://github.com/equinor/sepes-api/commit/ea8d1df73a0cc3efed7d65ed1ccc72efb41cde58))
* **config:** Fixed typo for db connection string in confg and key vaulkt ([5226dc4](https://github.com/equinor/sepes-api/commit/5226dc47546a1f0335cef5c235dbc66913d00dd4))
* **cypress:** use group access from token for mock user ([#691](https://github.com/equinor/sepes-api/issues/691)) ([7c84d67](https://github.com/equinor/sepes-api/commit/7c84d675e9c0c321c74edaa8c91a6d3482f05a01))
* **dataset:** add properties key and last modified to filelist ([#655](https://github.com/equinor/sepes-api/issues/655)) ([db73634](https://github.com/equinor/sepes-api/commit/db73634a13b2b548da2e4ca678242bb57bc34096))
* **dataset:** added permissions to study specific dataset response (c… ([#471](https://github.com/equinor/sepes-api/issues/471)) ([1fb5b4e](https://github.com/equinor/sepes-api/commit/1fb5b4e42f5fc9faf451f0bc69b254a88687f85d))
* **dataset:** assign custom role for dataset resource group ([#699](https://github.com/equinor/sepes-api/issues/699)) ([9a4d9af](https://github.com/equinor/sepes-api/commit/9a4d9af4e5c1389ac3b2b6039307cbc9fddb4c8d)), closes [#692](https://github.com/equinor/sepes-api/issues/692)
* **dataset:** better handling of missing resource group edge case ([#601](https://github.com/equinor/sepes-api/issues/601)) ([94fa316](https://github.com/equinor/sepes-api/commit/94fa31623a8904d6473c9394e061abeb1390ccd7))
* **dataset:** delete not working ([#654](https://github.com/equinor/sepes-api/issues/654)) ([32f6de9](https://github.com/equinor/sepes-api/commit/32f6de9ebea4ce7819552fb841e16c520f5b6992))
* **dataset:** ensuring container exist when getting file list ([#597](https://github.com/equinor/sepes-api/issues/597)) ([e202ea3](https://github.com/equinor/sepes-api/commit/e202ea33c588d801ae8d05e20352fe309df0ba71))
* **dataset:** ensuring rg exist on every dataset create ([d6cd6d9](https://github.com/equinor/sepes-api/commit/d6cd6d9258c5890656b8b74912c00fb2f05f3c08))
* **dataset:** getting server ip retries 3 times and caching result ([#586](https://github.com/equinor/sepes-api/issues/586)) ([ce8fd96](https://github.com/equinor/sepes-api/commit/ce8fd961be0fc7dec5333a0ded9780821f92dd4a))
* **dataset:** increased dataset max upload size ([23be629](https://github.com/equinor/sepes-api/commit/23be62957e9b50fdae780dea438dee3e6abfec35))
* **dataset:** increased upload sas token timeout to 30m ([64360c4](https://github.com/equinor/sepes-api/commit/64360c4a5b9b1059e92b5b08889339bb752dbc95))
* **datasetmodel:** changed studyno to studyid ([aad03bd](https://github.com/equinor/sepes-api/commit/aad03bde9a139751bc545f7fcd2f0f361f9d61ad))
* **dataset:** new sas token endpoing for deleting files ([#592](https://github.com/equinor/sepes-api/issues/592)) ([b818d10](https://github.com/equinor/sepes-api/commit/b818d1082b1ac00015e9a2c24edb4e0f9e7ec646)), closes [#590](https://github.com/equinor/sepes-api/issues/590)
* **dataset:** now adding correct client ip firewall rule ([ce3c464](https://github.com/equinor/sepes-api/commit/ce3c464dea5bb86093daf51cdef9986f06f2fcff))
* **dataset:** now checking study specific access when reading dataset ([#533](https://github.com/equinor/sepes-api/issues/533)) ([06f40fc](https://github.com/equinor/sepes-api/commit/06f40fcea5fe8aa11e27f269e57159420f988d67)), closes [#532](https://github.com/equinor/sepes-api/issues/532)
* **dataset:** now passing correct arguments to role assignment service ([#492](https://github.com/equinor/sepes-api/issues/492)) ([e27ab4c](https://github.com/equinor/sepes-api/commit/e27ab4c13d7e6e49d32859a3856cdc407820c974))
* **datasetservice, datasetcontroller:** studySpecificDatasets not available from datasetcontroller ([6e0ce98](https://github.com/equinor/sepes-api/commit/6e0ce98b49235d64b02b089a8da94e3bcbf68c49))
* **datasetservice:** added check to see if StorageAccountName is supplied ([bcbb9a4](https://github.com/equinor/sepes-api/commit/bcbb9a4812ec38f557bd16c79525eea71f9af12b))
* **dataset:** set azure role assignments for storage account and resource group ([#513](https://github.com/equinor/sepes-api/issues/513)) ([0949136](https://github.com/equinor/sepes-api/commit/09491361e13b9646355cefda0b4d3bab163cedc0))
* **datasetsrevice:** forgot to register datasetservice ([f3c80fb](https://github.com/equinor/sepes-api/commit/f3c80fb6b69ad11bad47c99b3bf315dfe385c4b1))
* **DatasetUtils:** add nullcheck ([#618](https://github.com/equinor/sepes-api/issues/618)) ([1e64848](https://github.com/equinor/sepes-api/commit/1e6484821d9c17438fe41ed1b96a69cb6273002d))
* **deployazureresourcesdidnotbuild:** forgot to comment out some temp code tpo make it build ([dddf886](https://github.com/equinor/sepes-api/commit/dddf886a70128264e3998a463513ccaa9ef84455))
* **docker:** Changed expose port to 80 instead of 5001. Modified docker-compose after splitting repos. ([cb43f8e](https://github.com/equinor/sepes-api/commit/cb43f8e1d6f288b4ee8dbea0db1e3c1474e0bb9d))
* ensured relevant tags are being set for all resources ([309eba4](https://github.com/equinor/sepes-api/commit/309eba4c65d624b5f44156e268bd026242a11c6f))
* filter out dash in storage account name ([#736](https://github.com/equinor/sepes-api/issues/736)) ([63592a0](https://github.com/equinor/sepes-api/commit/63592a08b2b13a363dbb59b9f585d7c65bb36943))
* **fixed argument to method with wrong name:** error occured when merging, sending wrong arg name ([e117e96](https://github.com/equinor/sepes-api/commit/e117e96709bbe89212ab3bb2678e48bb078e738c))
* **function:** run as managed identity ([#700](https://github.com/equinor/sepes-api/issues/700)) ([3b50e56](https://github.com/equinor/sepes-api/commit/3b50e56161a181ae63ae3df45dc66045cb05ed7a)), closes [#669](https://github.com/equinor/sepes-api/issues/669)
* **function:** updated managed by ids for dev and prod ([#663](https://github.com/equinor/sepes-api/issues/663)) ([03d44d6](https://github.com/equinor/sepes-api/commit/03d44d6f5dfbc51507b454dd292de166f49fc01c))
* **IHasLogoUrl:** Removed public in interface ([d974a00](https://github.com/equinor/sepes-api/commit/d974a00b5694b59efbba01d22711eedf9a661fc8))
* issue where space was not allowed in study, sandbox and vm name ([f77ccb8](https://github.com/equinor/sepes-api/commit/f77ccb85e37d041b53ca01aed880dfab5981f3e7))
* **keyvault:** clean old passwords, changed created filter to use utc ([a31feb8](https://github.com/equinor/sepes-api/commit/a31feb8d371d4d3432be885aa11e95058ff80e56))
* **logging:** clean up appi event ids ([#670](https://github.com/equinor/sepes-api/issues/670)) ([869ccca](https://github.com/equinor/sepes-api/commit/869ccca26774791556506cae752d37740ba21cb6))
* **logging:** dont log all provisioning exceptions as exceptions to App Insights ([#498](https://github.com/equinor/sepes-api/issues/498)) ([037e4c7](https://github.com/equinor/sepes-api/commit/037e4c7fd530adeaf82c7631efeb36f0139b886f))
* **logging:** Fixed typo in StudyController ([7430a0a](https://github.com/equinor/sepes-api/commit/7430a0a0d4a6865be827c93dc5c170dbc8440dd6))
* **make it build:** project did not build on last attempt ([c9d7336](https://github.com/equinor/sepes-api/commit/c9d7336e4144bdaf2ddfcf1a75702137879ac9bc))
* **minor:** changed extension for radixconfig ([a6d635c](https://github.com/equinor/sepes-api/commit/a6d635c76a454afaefcff5e8ad796b46e01dc05d))
* **monitoring:** improved log messages, added standardized eventids ([#580](https://github.com/equinor/sepes-api/issues/580)) ([37cf7f4](https://github.com/equinor/sepes-api/commit/37cf7f4515e6656ea871c68fba464f03a293750a))
* New endpoint for study roles where it will return which role a u… ([#560](https://github.com/equinor/sepes-api/issues/560)) ([673283b](https://github.com/equinor/sepes-api/commit/673283b7980f6a60e0dbdbc93faae2cdfdeb1e3a))
* **partcipants endpoint:** changed back to /api on endpoints ([bc68132](https://github.com/equinor/sepes-api/commit/bc6813243aa61c5bc9ce3e46670d74aac1d8682e))
* participant search from employees and affiliates ([#701](https://github.com/equinor/sepes-api/issues/701)) ([35f8a80](https://github.com/equinor/sepes-api/commit/35f8a80b82d2afc74a436a2338a140976414bd44))
* **participant:** added fields that were missing. Role is still missing ([1ce426e](https://github.com/equinor/sepes-api/commit/1ce426ea7eb91aaa008fcc89fc973e8b597a6b2f))
* **participantdtomissingfield:** participatDto was missing UserName and EmailAddress ([14969fe](https://github.com/equinor/sepes-api/commit/14969fe63821f4bbfd327a54c8f47f615f031927))
* **participant:** fixed error in participant lookup where same role was added multiple times ([#567](https://github.com/equinor/sepes-api/issues/567)) ([c8d79c9](https://github.com/equinor/sepes-api/commit/c8d79c99b2970e80b17e1b544e28247b932ae911))
* **participant:** remove created by filter on existing role assignments before add ([#667](https://github.com/equinor/sepes-api/issues/667)) ([47c8663](https://github.com/equinor/sepes-api/commit/47c8663c6add37b6f2b6a2399f59a70af8c49888))
* **participants:** issue where sepes was dependent on Azure to return… ([#502](https://github.com/equinor/sepes-api/issues/502)) ([f34f980](https://github.com/equinor/sepes-api/commit/f34f980bcd635de0832ab4f2b0ef42dfda7e7448)), closes [#499](https://github.com/equinor/sepes-api/issues/499)
* prevent fail in resource naming  ([#709](https://github.com/equinor/sepes-api/issues/709)) ([7345f92](https://github.com/equinor/sepes-api/commit/7345f9253db34eee3a8beb670191f6ec94b2cb21)), closes [#707](https://github.com/equinor/sepes-api/issues/707)
* prevent timeout when getting studies ([#645](https://github.com/equinor/sepes-api/issues/645)) ([44e329b](https://github.com/equinor/sepes-api/commit/44e329b13348fd87ec306792f4199c3f4444d24f))
* **radix:** Added logging for usehttpsredirection ([05aae97](https://github.com/equinor/sepes-api/commit/05aae97e50e4f6558d79480e7def93fba2c82af6))
* **radixconfig:** Updated APPI key in radixconfig ([41f84c6](https://github.com/equinor/sepes-api/commit/41f84c6cc2578d129a6c45c13aebba9f5eb8bc35))
* **radix:** Moved enivronment variables to backend secton, ahd palced them in front end ([154535b](https://github.com/equinor/sepes-api/commit/154535baf91cf0bff952c1b3e65586f4fdd508eb))
* **radix:** Playing with ports in radixconfig ([2386b60](https://github.com/equinor/sepes-api/commit/2386b60f4802057fd4080fdb906c5d3a6a44679c))
* **radix:** Set httonly to true in radixconfig ([99f3418](https://github.com/equinor/sepes-api/commit/99f3418817f71d6696980d7fe430e6bbbbfe52dc))
* **radix:** Update radixconfig with new application name ([2d32bd6](https://github.com/equinor/sepes-api/commit/2d32bd6dabf25d4b29bd79b67048810818257120))
* **rbac:** added missing permissions for admin ([#445](https://github.com/equinor/sepes-api/issues/445)) ([ea8a189](https://github.com/equinor/sepes-api/commit/ea8a189d4b8ee9a576ccefc685227bfac9e42a5b))
* **rbac:** added role for employee vs external user. more unit tests.… ([#440](https://github.com/equinor/sepes-api/issues/440)) ([12f44a6](https://github.com/equinor/sepes-api/commit/12f44a6c0f4ab97917f7c0eb9bd69ce3eb9e044f))
* **rbac:** admin was still missing some permissions that only he shold have ([37087f1](https://github.com/equinor/sepes-api/commit/37087f1f1beffccd05739386995acd5ef1080913))
* **rbac:** now allowing b2c user access ([#531](https://github.com/equinor/sepes-api/issues/531)) ([ac61458](https://github.com/equinor/sepes-api/commit/ac61458549a557960b9e7f83c08391adb8b6efb5))
* reduce max length of description ([3bc80bc](https://github.com/equinor/sepes-api/commit/3bc80bc41b5a64874291a20a4e0fff9da147d797))
* **remove participant:** fixed lambda expression ([141f3c1](https://github.com/equinor/sepes-api/commit/141f3c1e74ec45f0413d0144fad0924bc33daa28))
* renamed dataset properties in permissions response. Also now using rbac engine ([#432](https://github.com/equinor/sepes-api/issues/432)) ([895b713](https://github.com/equinor/sepes-api/commit/895b713f5607c68dfaca133a2b4ca3bb93cf393b))
* Require names of studies, vms and sandboxes to have 3 or more ch… ([#509](https://github.com/equinor/sepes-api/issues/509)) ([bd169eb](https://github.com/equinor/sepes-api/commit/bd169eb95e37aba0f428ddc9c7c2c6857d6dc498)), closes [#495](https://github.com/equinor/sepes-api/issues/495)
* **resourcegroupnaming:** ensured that naming of resourceGroups are consistent and pseudo-unique ([511c578](https://github.com/equinor/sepes-api/commit/511c578c49ee30a9764f4d18e8002fd9e02a3d0a))
* **sandbox creation:** ensuring study and sandbox name part of resour… ([#401](https://github.com/equinor/sepes-api/issues/401)) ([fe26189](https://github.com/equinor/sepes-api/commit/fe26189c902994850f36bcd0dbf4198a3a4f521d))
* **sandbox creation:** rule priority for basic nsg rule was to high, now set to 4050 ([946009f](https://github.com/equinor/sepes-api/commit/946009fbe688366f5058df1270331eec6e1e6290))
* **sandbox resource list:** fixed when status is showing create failed for deleted resources ([0766619](https://github.com/equinor/sepes-api/commit/07666193158f18f18a2373d1455f71b47595b1bc))
* **sandbox:** add correct dataset restrictions text ([3b3afd8](https://github.com/equinor/sepes-api/commit/3b3afd839d85af59949cd9c4ae59eb1829e66c68))
* **sandbox:** add/remove dataset. Improved validation and messages. Now returning HTTP 204 on succes ([d6f534c](https://github.com/equinor/sepes-api/commit/d6f534cbce43ff1794b81605c3e87cf343abf8dc))
* **sandbox:** added null checks to resource endpoint ([607d8db](https://github.com/equinor/sepes-api/commit/607d8dba15105ae5d7125df00f69e025f6321d03))
* **sandbox:** attempt to prevent fail of role assignment update task ([#649](https://github.com/equinor/sepes-api/issues/649)) ([fe12b58](https://github.com/equinor/sepes-api/commit/fe12b58e70bb7e7d6246303139d779c33b578a73))
* **sandbox:** better null handling and logging in vnet creation ([#610](https://github.com/equinor/sepes-api/issues/610)) ([bbc7238](https://github.com/equinor/sepes-api/commit/bbc723859b8bb19dfcb903397720ce1dd5b26699))
* **sandbox:** changed when resource retry link is created ([#600](https://github.com/equinor/sepes-api/issues/600)) ([baec250](https://github.com/equinor/sepes-api/commit/baec250c264a77894b51da3ac1d2cae228122281))
* **sandbox:** fixed mapping error occuring in sandbox response. ([#582](https://github.com/equinor/sepes-api/issues/582)) ([e01224f](https://github.com/equinor/sepes-api/commit/e01224f23004596deef11978ab9bec36277dc224))
* **sandbox:** fixed mapping in resource response ([#568](https://github.com/equinor/sepes-api/issues/568)) ([a8673a4](https://github.com/equinor/sepes-api/commit/a8673a4937b668c41d6a591b9c8fb82210094787))
* **sandbox:** Include disk price to the estimated cost of an vm ([#469](https://github.com/equinor/sepes-api/issues/469)) ([cd06148](https://github.com/equinor/sepes-api/commit/cd06148be05760c7a6706c4d9d2dde1252d7aaf3))
* **sandbox:** new endpoint for getting cost analysis ([1f8d837](https://github.com/equinor/sepes-api/commit/1f8d8374dd41b3f4b76c4592ede730a760c2d74a))
* **sandbox:** removed duplicate check of existing name ([71a2abc](https://github.com/equinor/sepes-api/commit/71a2abc5b28ab3671ff5ffb3c00bc00884a1116f))
* **sandboxservice:** sandboxService now throws NotFoundException if sandbox is not found ([cb956ae](https://github.com/equinor/sepes-api/commit/cb956aefc75d92902fb4c653b3aea6617e74cb54))
* **sandbox:** sort sizes by price with lowest cost first ([#462](https://github.com/equinor/sepes-api/issues/462)) ([f56f01d](https://github.com/equinor/sepes-api/commit/f56f01d9402cc78d257452e9a4aea9e811a17b50)), closes [#423](https://github.com/equinor/sepes-api/issues/423)
* **sandbox:** timeout when going to next phase ([#621](https://github.com/equinor/sepes-api/issues/621)) ([b4b901a](https://github.com/equinor/sepes-api/commit/b4b901abeac65c2c089f99566c338caaded61ef1))
* **sepesdbcontext:** now adds datetime to Created and Updated field on Resource and Operations ([3ca8bf7](https://github.com/equinor/sepes-api/commit/3ca8bf7820153b9cc8bdb610b31dcccefa6ca970))
* standardized how azure services handles non existing resources ([#599](https://github.com/equinor/sepes-api/issues/599)) ([13d565f](https://github.com/equinor/sepes-api/commit/13d565f4c8eb204ef6b656a43d6c37253c014113))
* **startup.cs:** changed JSON formatting back to camelCase ([6a7d3cf](https://github.com/equinor/sepes-api/commit/6a7d3cfcff8ee19839365b6736665a196ee44ffe))
* **startup:** better logging for empty db con string ([d40427f](https://github.com/equinor/sepes-api/commit/d40427f48a5337d24ec74f692a0ca7c914a65ee0))
* **startup:** Renamed Appi key with SEPES_ prefix. Added some logg ing to program.cs and Starup.cs ([550f259](https://github.com/equinor/sepes-api/commit/550f259650776142fdcbd1940b5826f3bc01480d))
* **studies:** external users not  seeing all studies in list anymore ([#537](https://github.com/equinor/sepes-api/issues/537)) ([3bd15bd](https://github.com/equinor/sepes-api/commit/3bd15bd09f60173aecaf9f2b0b41ca63992f207e))
* **study create:** was failing due to missing permission check ([#376](https://github.com/equinor/sepes-api/issues/376)) ([7a01d95](https://github.com/equinor/sepes-api/commit/7a01d95b3d8cc05a834859d5b243439c961cd645))
* **study details:** missing include causing mapping error for StudyDatasets > SandboxDataset ([#414](https://github.com/equinor/sepes-api/issues/414)) ([9ae327c](https://github.com/equinor/sepes-api/commit/9ae327c3ccfb1452e32928b2b7d9a09a45003dc1))
* **study- service and controller, startup:** completed merge, and fixed errors ([f2b5ce2](https://github.com/equinor/sepes-api/commit/f2b5ce2900a8e533110b55efc231677b09302813))
* **study:** add and remove participant did not save ([a78d8e4](https://github.com/equinor/sepes-api/commit/a78d8e47abded38395a065bc6a60ebf831317271))
* **studycontroller:** added FromForm to fix headers ([4860e09](https://github.com/equinor/sepes-api/commit/4860e09c0de27868e828027401ffcf4b83554ee2))
* **studycontroller:** added missing "/" in Put method for changing datasets ([b26842e](https://github.com/equinor/sepes-api/commit/b26842e7241c952243aa6630265799af519fb0db))
* **study:** delete failed when no dataset resource group was set ([#444](https://github.com/equinor/sepes-api/issues/444)) ([ff30438](https://github.com/equinor/sepes-api/commit/ff30438fa862549897018be534b729462e7078d7))
* **study:** delete sometimes failed when no resource group created ([#525](https://github.com/equinor/sepes-api/issues/525)) ([558c483](https://github.com/equinor/sepes-api/commit/558c483028108426613a45686cbd064a8e8dd5e9))
* **study:** details response also including datasets not in any sandbox ([#695](https://github.com/equinor/sepes-api/issues/695)) ([c3a972a](https://github.com/equinor/sepes-api/commit/c3a972a7756e8e5eab1f3bb0072463ac10daf6af)), closes [#694](https://github.com/equinor/sepes-api/issues/694)
* **studyDetails:** added which sandboxes a given datasets is used in ([#403](https://github.com/equinor/sepes-api/issues/403)) ([d960358](https://github.com/equinor/sepes-api/commit/d960358bab05d8cc9b3a2c02281698375c831adb))
* **studydtohassasforlogoevenwhennologo:** study dto got sas token even when there was no logo url ([86591a1](https://github.com/equinor/sepes-api/commit/86591a1306e5189d2c8ea1d06f478296c948e80d))
* **studydtologourlsas:** studyDto did not get sas for logo url. Now fixed ([53bcb56](https://github.com/equinor/sepes-api/commit/53bcb56e5a9cddec4bdbc67d38661d131fc2eed3))
* **study:** fix slow study close due to include ([#668](https://github.com/equinor/sepes-api/issues/668)) ([e1628fd](https://github.com/equinor/sepes-api/commit/e1628fd1a738c26d352da41e2ee7f496b1208929))
* **study:** increase logo sas token timeout to 61 min ([#726](https://github.com/equinor/sepes-api/issues/726)) ([00c5a6f](https://github.com/equinor/sepes-api/commit/00c5a6f19e266ddafc700143c0d339829c3a5b72))
* **study:** increase performance for GET resultsandlearnings ([#607](https://github.com/equinor/sepes-api/issues/607)) ([8bdedc5](https://github.com/equinor/sepes-api/commit/8bdedc5d6ef520124cb0e48c62046d9c25de5d10))
* **study:** issue with role list and admin role ([#716](https://github.com/equinor/sepes-api/issues/716)) ([15bcc16](https://github.com/equinor/sepes-api/commit/15bcc163dddb225782ca6ea9a7a9e4d4dc6cbc1e)), closes [#710](https://github.com/equinor/sepes-api/issues/710)
* **studymodel:** vendor and WBS did not get saved ([6907801](https://github.com/equinor/sepes-api/commit/6907801ce36fd31d4065f30cd65504245dee81b9))
* **study:** now returning lighter response for study details endpoint ([15c4f02](https://github.com/equinor/sepes-api/commit/15c4f026938f0f563e7b70164d51abd283dc5593))
* **study:** now sponsor and sponsor rep are allowed to soft delete st… ([#653](https://github.com/equinor/sepes-api/issues/653)) ([1801f72](https://github.com/equinor/sepes-api/commit/1801f72cc9dbce3fd0c5ed6761c39589d0f2f986))
* **study:** participant search ([#723](https://github.com/equinor/sepes-api/issues/723)) ([66e0267](https://github.com/equinor/sepes-api/commit/66e0267298658966203b6a179d2fa6cc46f95a6c)), closes [#712](https://github.com/equinor/sepes-api/issues/712)
* **studyparticipant:** added back id of participant ([64d6526](https://github.com/equinor/sepes-api/commit/64d65264d239f0eb2319725c3c75e2e27bf98d2a))
* **studyparticipantwrongdto:** adde correct fields to studyparticipant ([6416f2c](https://github.com/equinor/sepes-api/commit/6416f2c70bfa6fb47024b34b183c8236b2feed7f))
* **study:** prevent setting invalid wbs if active sandbox or dataset ([#705](https://github.com/equinor/sepes-api/issues/705)) ([f909a21](https://github.com/equinor/sepes-api/commit/f909a211f6cdc97a388337245c44370834818491)), closes [#703](https://github.com/equinor/sepes-api/issues/703) [#704](https://github.com/equinor/sepes-api/issues/704)
* **study:** prevented details request deadlock ([#611](https://github.com/equinor/sepes-api/issues/611)) ([d2c0859](https://github.com/equinor/sepes-api/commit/d2c085977f8110135995befcecd4ac50384de629))
* **study:** remove logo if logoUrl is empty ([#589](https://github.com/equinor/sepes-api/issues/589)) ([60aa506](https://github.com/equinor/sepes-api/commit/60aa506d0c306a54b4ae14a144197644ee2d2e4c)), closes [#588](https://github.com/equinor/sepes-api/issues/588)
* **study:** returning the detailed response for CreateStudy, UpdateLogo and UpdateStudyDetails ([f5a4fb1](https://github.com/equinor/sepes-api/commit/f5a4fb1fef220032e5f6a8f83147e029d4bd93ea))
* **studyservice, studycontroller:** moved _config from controller to service ([9c69fbe](https://github.com/equinor/sepes-api/commit/9c69fbe8a7a56882bb2200a3dd5a394b5b6e34b8))
* **studyservice.cs:** when deleting study, method will now delete studylogo from Azure blob Storage ([94ffb5e](https://github.com/equinor/sepes-api/commit/94ffb5eab8d6ee60887a0c7988507a1fba03ae66))
* **studyservice:** updateStudyDetails now updates ResultsAndLearnings field ([af1bafd](https://github.com/equinor/sepes-api/commit/af1bafdf2df768feb165d62a3882f85371501bca))
* **study:** streamlined create and add logo ([#603](https://github.com/equinor/sepes-api/issues/603)) ([77efc2b](https://github.com/equinor/sepes-api/commit/77efc2bb02e7181f3cb779eefb16b386c1790e3c))
* **study:** wbs cache use dapper and isolate errors ([#724](https://github.com/equinor/sepes-api/issues/724)) ([ae49bfa](https://github.com/equinor/sepes-api/commit/ae49bfadd0e5ef278a88827e7db3b10df99f63c1)), closes [#720](https://github.com/equinor/sepes-api/issues/720)
* **stuyresourcegroupnameandtests:** fixed resource group name for studies. Some cleaning ([1f9da6d](https://github.com/equinor/sepes-api/commit/1f9da6d5ecc15500466c7ebeb460b110900c3be1))
* **technicalcontact info was missing on sandbox:** getting it from claimsprincipal ([8e12841](https://github.com/equinor/sepes-api/commit/8e1284104fcb63082374a4c15dc2e1281c34e417))
* **user util  unable to find username:** now added relevant scope on frontend. Can clean up here ([536e023](https://github.com/equinor/sepes-api/commit/536e023ee98d9b43f90e5e506de018a05d054dcc))
* Validate firewall IP rules ([1866644](https://github.com/equinor/sepes-api/commit/18666447832bbb0c0d8fcd514af345a9448a8349))
* **virtual machine:** resolve empty size lookup for some regions ([#402](https://github.com/equinor/sepes-api/issues/402)) ([b628869](https://github.com/equinor/sepes-api/commit/b628869538e1d61e883ce9caef43fbfb09f24865))
* **vm rules:** fixed issue where vm creation and update lost got off track ([#416](https://github.com/equinor/sepes-api/issues/416)) ([a8025b0](https://github.com/equinor/sepes-api/commit/a8025b08abd6ea77d2a542207aa5e9ca22e0c27e))
* **vm rules:** now keeping track of priorities, asking azure when creating rules ([#413](https://github.com/equinor/sepes-api/issues/413)) ([485ad82](https://github.com/equinor/sepes-api/commit/485ad82b8500dd5c1bba3d6621705687b992f9bc))
* **vm rules:** returning empty list instead of null, when no rules exist ([#379](https://github.com/equinor/sepes-api/issues/379)) ([7bf027d](https://github.com/equinor/sepes-api/commit/7bf027d37a97c64408108e6dbcd3ee33446d8de8))
* **vm:** add endpoint for validating a username. Some names and perio… ([#479](https://github.com/equinor/sepes-api/issues/479)) ([6394c24](https://github.com/equinor/sepes-api/commit/6394c24471930f99d8c4ca278f017a24f3f30962)), closes [#478](https://github.com/equinor/sepes-api/issues/478)
* **vm:** filtered away zrs disks ([#634](https://github.com/equinor/sepes-api/issues/634)) ([08e3ca3](https://github.com/equinor/sepes-api/commit/08e3ca331b08ba0da503bf0e00f6f5fd1fd69a10))
* **vm:** improved selection of price for all SKUs. Avoiding "low priority" and "spot" if possible ([acec3d4](https://github.com/equinor/sepes-api/commit/acec3d4099e7ed73ce2136c0a722da5cc0957df6))
* **vm:** issue with password validation: Updated test ([fc0577f](https://github.com/equinor/sepes-api/commit/fc0577f31478599862dc02b6ef4916e0cd63767f)), closes [#504](https://github.com/equinor/sepes-api/issues/504) [#503](https://github.com/equinor/sepes-api/issues/503)
* **vm:** new endpoint for getting price of an vm without calling azure ([43aa823](https://github.com/equinor/sepes-api/commit/43aa8232ce84385f9b3efeab3d84061dd59e2c87))
* **vm:** no public IP was created for VM ([#696](https://github.com/equinor/sepes-api/issues/696)) ([11f84e8](https://github.com/equinor/sepes-api/commit/11f84e89a2f4014f147715dc7ac184f1355f9d97)), closes [#693](https://github.com/equinor/sepes-api/issues/693)
* **vm:** now disables disk cache from 4tb ([#632](https://github.com/equinor/sepes-api/issues/632)) ([9047a14](https://github.com/equinor/sepes-api/commit/9047a140d4bedeb029dcc1f0a8d6ab72e9fcea1f))
* **vm:** now rolling back, if creation fails in the create response ([#490](https://github.com/equinor/sepes-api/issues/490)) ([91b87f9](https://github.com/equinor/sepes-api/commit/91b87f9bf33d042a1fa5d29673eb2a5f73e447b8))
* **vm:** potential fix for failed creation in other users sandbox ([ade6540](https://github.com/equinor/sepes-api/commit/ade6540d82081530a53d4772ff15b476e2fb625a))
* **vm:** provisioning can now resume if last attempt failed ([#491](https://github.com/equinor/sepes-api/issues/491)) ([084e030](https://github.com/equinor/sepes-api/commit/084e0300c02e320478ddde3781f98b48d151b029))
* **vm:** username validation now takes what os is picked into consideration ([2ff065d](https://github.com/equinor/sepes-api/commit/2ff065d6f6caa49dc4ac5c5c14dd60d0af5a3c46)), closes [#521](https://github.com/equinor/sepes-api/issues/521)
* **vm:** validate password and throw error if it does not meet requir… ([#476](https://github.com/equinor/sepes-api/issues/476)) ([2b1cd7b](https://github.com/equinor/sepes-api/commit/2b1cd7be5738676b5df138c04832a0bf02495192)), closes [#337](https://github.com/equinor/sepes-api/issues/337)
* **vm:** vm extended info endpoint, renamed MemoryInDb to MemoryGB ([#424](https://github.com/equinor/sepes-api/issues/424)) ([939cca7](https://github.com/equinor/sepes-api/commit/939cca7d7520f74defca8edd3feaac245a85fd31))
* **vm:** vm name validation can now handle special characters. For instance question mark ([#482](https://github.com/equinor/sepes-api/issues/482)) ([050f53a](https://github.com/equinor/sepes-api/commit/050f53af5cc20ac4f812464f4484edefb28da141)), closes [#481](https://github.com/equinor/sepes-api/issues/481)
* **vm:** vm not getting stuck in updating if previous attempt failed ([#470](https://github.com/equinor/sepes-api/issues/470)) ([b9e5685](https://github.com/equinor/sepes-api/commit/b9e568540e36e94ac8e0326a29bba2d5717cac16))
* **wbsvalidation:** better cache duplication handling ([#718](https://github.com/equinor/sepes-api/issues/718)) ([f32b3ff](https://github.com/equinor/sepes-api/commit/f32b3ffec4fecdea29bd0a6306ca459255f57fce)), closes [#717](https://github.com/equinor/sepes-api/issues/717)
* **worker:** add error to resource ([#516](https://github.com/equinor/sepes-api/issues/516)) ([80f98da](https://github.com/equinor/sepes-api/commit/80f98da82da898c2f4b541d169a955143530ea9c))
* **worker:** added trace logging in startup ([#620](https://github.com/equinor/sepes-api/issues/620)) ([7c86c3b](https://github.com/equinor/sepes-api/commit/7c86c3bbf3402518c78589f8bca2306c74a5269e))
* **worker:** decreased time taken for VM creation to start ([#463](https://github.com/equinor/sepes-api/issues/463)) ([f0fe1f7](https://github.com/equinor/sepes-api/commit/f0fe1f74fef8c320d871f3a203aa060891d52850))
* **worker:** lifespan of health service now transient ([#722](https://github.com/equinor/sepes-api/issues/722)) ([091db37](https://github.com/equinor/sepes-api/commit/091db3773dc66d17593b2ce68c537a3a09c54f36))
* **worker:** sandbox creation queue timeout and better in prog protection ([#613](https://github.com/equinor/sepes-api/issues/613)) ([1a0500e](https://github.com/equinor/sepes-api/commit/1a0500e06df9ba71358a0995b6ef44c8c8cabe00))


* feat(studyspecificdataset)/implement file upload (#443) ([6dcd4c4](https://github.com/equinor/sepes-api/commit/6dcd4c4663978a94f41a3739f81c29677dcf7227)), closes [#443](https://github.com/equinor/sepes-api/issues/443)
* Merge from develop to master (#364) ([34aa71f](https://github.com/equinor/sepes-api/commit/34aa71fe688e57c33fb3d6807029b76426c0a5e9)), closes [#364](https://github.com/equinor/sepes-api/issues/364) [#267](https://github.com/equinor/sepes-api/issues/267) [#272](https://github.com/equinor/sepes-api/issues/272) [#271](https://github.com/equinor/sepes-api/issues/271) [#274](https://github.com/equinor/sepes-api/issues/274) [#275](https://github.com/equinor/sepes-api/issues/275) [#276](https://github.com/equinor/sepes-api/issues/276) [#277](https://github.com/equinor/sepes-api/issues/277) [#279](https://github.com/equinor/sepes-api/issues/279) [#280](https://github.com/equinor/sepes-api/issues/280) [#281](https://github.com/equinor/sepes-api/issues/281) [#282](https://github.com/equinor/sepes-api/issues/282) [#285](https://github.com/equinor/sepes-api/issues/285) [#287](https://github.com/equinor/sepes-api/issues/287) [#295](https://github.com/equinor/sepes-api/issues/295) [#294](https://github.com/equinor/sepes-api/issues/294) [#297](https://github.com/equinor/sepes-api/issues/297) [#296](https://github.com/equinor/sepes-api/issues/296) [#298](https://github.com/equinor/sepes-api/issues/298) [#293](https://github.com/equinor/sepes-api/issues/293) [#299](https://github.com/equinor/sepes-api/issues/299) [#302](https://github.com/equinor/sepes-api/issues/302) [#301](https://github.com/equinor/sepes-api/issues/301) [#304](https://github.com/equinor/sepes-api/issues/304) [#306](https://github.com/equinor/sepes-api/issues/306) [#308](https://github.com/equinor/sepes-api/issues/308) [#307](https://github.com/equinor/sepes-api/issues/307) [#319](https://github.com/equinor/sepes-api/issues/319) [#317](https://github.com/equinor/sepes-api/issues/317) [#318](https://github.com/equinor/sepes-api/issues/318) [#321](https://github.com/equinor/sepes-api/issues/321) [#322](https://github.com/equinor/sepes-api/issues/322) [#323](https://github.com/equinor/sepes-api/issues/323) [#313](https://github.com/equinor/sepes-api/issues/313) [#328](https://github.com/equinor/sepes-api/issues/328) [#310](https://github.com/equinor/sepes-api/issues/310) [#330](https://github.com/equinor/sepes-api/issues/330) [#332](https://github.com/equinor/sepes-api/issues/332) [#333](https://github.com/equinor/sepes-api/issues/333) [#335](https://github.com/equinor/sepes-api/issues/335) [#338](https://github.com/equinor/sepes-api/issues/338) [#339](https://github.com/equinor/sepes-api/issues/339) [#340](https://github.com/equinor/sepes-api/issues/340) [#344](https://github.com/equinor/sepes-api/issues/344) [#341](https://github.com/equinor/sepes-api/issues/341) [#348](https://github.com/equinor/sepes-api/issues/348) [#352](https://github.com/equinor/sepes-api/issues/352) [#353](https://github.com/equinor/sepes-api/issues/353) [#355](https://github.com/equinor/sepes-api/issues/355) [#356](https://github.com/equinor/sepes-api/issues/356) [#358](https://github.com/equinor/sepes-api/issues/358) [#362](https://github.com/equinor/sepes-api/issues/362) [#361](https://github.com/equinor/sepes-api/issues/361) [#309](https://github.com/equinor/sepes-api/issues/309) [#363](https://github.com/equinor/sepes-api/issues/363)
* **deployazureresources:** cleaning up some names, unused usings and comments ([92b0610](https://github.com/equinor/sepes-api/commit/92b06102c49d06236a0793d8b177c2b6cbc11235))
* improve study read rbac coverage ([#714](https://github.com/equinor/sepes-api/issues/714)) ([b031107](https://github.com/equinor/sepes-api/commit/b031107d8b7ed3c0b9fa524f2b625e14ae19ee0f)), closes [#713](https://github.com/equinor/sepes-api/issues/713)
* removed sandbox endpoints that were simplified to not include/study/studyId part ([#437](https://github.com/equinor/sepes-api/issues/437)) ([cde78c0](https://github.com/equinor/sepes-api/commit/cde78c0d46cfcea20467f3237dad6342e620030a))
* results and learnings and study specific datasets ([#746](https://github.com/equinor/sepes-api/issues/746)) ([ccd6da9](https://github.com/equinor/sepes-api/commit/ccd6da9852b44263401b4ed6688d706717cf0f68)), closes [#742](https://github.com/equinor/sepes-api/issues/742) [#744](https://github.com/equinor/sepes-api/issues/744)

## 0.7.0 (2021-08-24)


### ⚠ BREAKING CHANGES

* url for deleting study specific dataset is now on api/studies/datasets/studyspecific/{datasetId}
* Removed support for CRUD operations for pre-approved datasets

* test: DatasetFileService
* Study details response does not contain resultsAndLearnings property

* fix: now able to get study even though logo functionality is failing

previously, all methhods for study was failing if something went wrong with logo

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

* feat(dataset): implemented file upload for study specific datasets

Also updated relevant unit tests
* three endpoints described above are replaced by endpoints not requiring the
studies/studyId part
* Method HandleAddParticipantAsync now returns StudyParticipant entry instead of
Study entry
* **deployazureresources:** Now requires admin for all endpoints
* **deployazureresources:** New DTO/form for creating sandbox. Only name, region and template needed. Template
is still not supported, so the field
* **sandboxservices:** Using api to create Sandbox now requires valid azure credentials to work properly,
as it now does stuff in azure

https://dev.azure.com/equinor-core-dev/Sepes/_boards/board/t/DevOps/Stories/?workitem=1856
* **azureblobstorageservice:** need To have azure container named studyLogos
* **dataset, studycontroller, studyservice, automapperconfig:** Migration, adds a field in datab
* **studycontroller:** Swagger is broken. Cant upload image there anymore
* **studyservice and logo:** Migration: Remove StudyLogos Table, also changed fields in Study. Remove LogoId and
added LogoUrl.
* **datamodel:** Data model for study changed

### Features

* **ad azure user search:** added endpoint to get users by searchstring ([779919b](https://github.com/equinor/sepes-api/commit/779919b8892308c489bd07809eeaa78f91cd98ce))
* **add swagger:** adding swagger. CHanged launchSettings so VS starts app on Swagger page ([1c75d56](https://github.com/equinor/sepes-api/commit/1c75d5606c288749295a9ec4f445bf069d4c668f))
* add wbs validation from external api ([#677](https://github.com/equinor/sepes-api/issues/677)) ([f607cf9](https://github.com/equinor/sepes-api/commit/f607cf9e976888ec6e57297ff062181a57d3ba6a))
* **added tag functionality:** added tag functionality and pulling together most relevant tags ([91c0377](https://github.com/equinor/sepes-api/commit/91c03776f50a5ad30fc41fd292038f38712713ab))
* **addshellfornewstudyendpoints:** added todo comments and enpty implementations ([90e5d00](https://github.com/equinor/sepes-api/commit/90e5d006eb832a4782a4fde71c8104167d6ad889))
* **adduserservice:** added a user service instead of the claims principal service ([0a2802f](https://github.com/equinor/sepes-api/commit/0a2802f31ee05ffd07acec4bfc4919ec2b6fdc75))
* **all azure services and interfaces:** added Exists(), made stgAccount check name availability ([9998545](https://github.com/equinor/sepes-api/commit/99985457f2dafb158e1ae0665b98407e5d62905e))
* **auth:** Progress. Replaced add auth and token validation. Had to add values to appsettings. will probably have to clean this out ([a9dd209](https://github.com/equinor/sepes-api/commit/a9dd20970fce6adc3ae3f5234aa71f23845ad24c))
* **azure search users:** fixed up the merge from develop. merge gone bad ([a295063](https://github.com/equinor/sepes-api/commit/a29506343f40d7d90cf5d65cdb4050f2225956c0))
* **azure search:** radixconfig, added AzureAd__ prefix ([d09111f](https://github.com/equinor/sepes-api/commit/d09111f4e627956963a3d7fa8414a8db61792dfc))
* **azure worker for sandbox resources:** some initial refactoring of services ([8c8db41](https://github.com/equinor/sepes-api/commit/8c8db41f04c853127649c0e1298a64b8fc1e147b))
* **azure worker:** endpoint for resources. Now updates provsisioning state after creation ([877ea2c](https://github.com/equinor/sepes-api/commit/877ea2c25792b94a2f5c7ccea796566e0d6736d4))
* **azure worker:** implemented create for all basic sandbox resource ([952c1e5](https://github.com/equinor/sepes-api/commit/952c1e5709fef2bba35b1f6c03347d0732a79d4c))
* **azure worker:** now also binds nsg to subnet ([8edd385](https://github.com/equinor/sepes-api/commit/8edd3854d30d6daa4388302131517d051c95e8af))
* **azure worker:** redefined how work is pulled. Cleaned DTOs. Redefined queue ([2589b2c](https://github.com/equinor/sepes-api/commit/2589b2c6e538ce6fd858f219a53895f0442f5502))
* **azure worker:** redefined how work is pulled. Cleaned DTOs. Redefined queue ([1f8b82a](https://github.com/equinor/sepes-api/commit/1f8b82ab8146f7299e6f59e846efa378ed635aed))
* **azure worker:** refactor and make it testable. Mocking azure resource group creation ([a258955](https://github.com/equinor/sepes-api/commit/a258955ee3edcf9355621cc2f32d324dc5c850f8))
* **azuread:** Refactored azure ad authentication. Now uses environment variables ([cf0860e](https://github.com/equinor/sepes-api/commit/cf0860e04530d72f8bd7b517c7b3a31ac3c60535))
* **azurebastionservice:** now creates a Bastion succesfully ([fbf4c12](https://github.com/equinor/sepes-api/commit/fbf4c1202a69a8b221991ffd6e67c6a1ee6f6df9))
* **azurequeueinterface:** updated types of AzureQueueInterface ([38ef6c1](https://github.com/equinor/sepes-api/commit/38ef6c1b3095464841ca51de317d1d626861ce0d))
* **azurequeueservice + tests:** made test class for QueueService, and improved functionality ([753006b](https://github.com/equinor/sepes-api/commit/753006b8e98da12daea733f9bccf31279c128454))
* **azurequeueservice, appsettings.json:** started writing AzureQueueService, Added Connectionstring ([5a7e67c](https://github.com/equinor/sepes-api/commit/5a7e67c8e6b8f339e005607d9749f5be1f2c666b))
* **azurequeueservice, iazurequeueservice:** azureQueueService is now implemented ([65d0380](https://github.com/equinor/sepes-api/commit/65d0380effc20a155ddae64a9736d3408e975b47))
* **azureresourcemonitoringservice, ihasexists:** created initial method for checking orphan resourc ([71206f0](https://github.com/equinor/sepes-api/commit/71206f04028dd5061faebd3f4a6b641d28b7c4a1))
* **azureresourcemonitoringservice:** added Tag check/update to resource monitor ([051baeb](https://github.com/equinor/sepes-api/commit/051baeb72d95306092850516e8f0afc20d11c828))
* **azureresourcenameutil:** added method for creating name for storagAccounts ([1b71c95](https://github.com/equinor/sepes-api/commit/1b71c95f26c0929a94e411461195f13359335fdc))
* **azureresourcenameutil:** naming of ResourceGroups now ensure uniqueness ([5f3cfba](https://github.com/equinor/sepes-api/commit/5f3cfba66b516eced1515537359ac3e3f8bde79f))
* **azureresourcenameutils:** add methods for cleaning sandboxName. Also made sure stgAcc are unique ([da62255](https://github.com/equinor/sepes-api/commit/da62255b145414fa947b02cf02f433ecae591b85))
* **azureservice, azureresourcegroupservice:** implemented NukeUnitTestSandboxes() ([a7e6a30](https://github.com/equinor/sepes-api/commit/a7e6a3036dcea0a5bd85a0d63a905be8a385160a))
* **azureservicetest:** progress on azureservice test, but not working ([c6779c7](https://github.com/equinor/sepes-api/commit/c6779c7e0abaf511e2afdc7761d204b25e00e67a))
* **azurestorageaccountservice:** beginning of a storageAccountService ([2701fd4](https://github.com/equinor/sepes-api/commit/2701fd42e668dddf57a269f4fdf455a6aea29336))
* **azurevmservice:** can now create a Windows VM ([54cfba6](https://github.com/equinor/sepes-api/commit/54cfba641ef692b7ab6430be60015b04226d0c21))
* **azurevmservice:** initial commit of work on AzureVService ([3f552e1](https://github.com/equinor/sepes-api/commit/3f552e1390ca78ea83ff42512533ae9b926dd196))
* **azurevmutils, azurevmservice, azureservice:** can now create VMs, with Boot diagnostics ([33a0b58](https://github.com/equinor/sepes-api/commit/33a0b58894581e633b316baf853a42d3fe8d68a7))
* **azureworker:** now scheduling work into queue and picking up on the other side ([51730bc](https://github.com/equinor/sepes-api/commit/51730bc21cbb388c29311ff094305abdae7ed5d1))
* **changed default log level to info:** changed log level ([60f29ec](https://github.com/equinor/sepes-api/commit/60f29ec866d3e2520c07da47b3ad0412405cf5b3))
* **changed lifecycle to user service to scoped:** userService and IPrincipalService is now Scoped ([f8c958f](https://github.com/equinor/sepes-api/commit/f8c958f85f5087c4bb024fa9c62810b33374a70d))
* changing auth to api auth ([9196ff2](https://github.com/equinor/sepes-api/commit/9196ff214914ab59d3767ac6cf6b893b86e13313))
* connected Study with Sandbox and DataSet ([c605056](https://github.com/equinor/sepes-api/commit/c6050560c4d25552d003e2c2b45b5e1bacae9868))
* **createsandboxbetterresult:** added missing GetSandbox endpoint and service method ([b182af6](https://github.com/equinor/sepes-api/commit/b182af6e39d352535af0bfc2c7bf3657872dac9b))
* **database:** Enabled EF Core migrations. Now using separate connection string for migrations vs other DB IO ops. ([55daefb](https://github.com/equinor/sepes-api/commit/55daefb2c2b55ae712b982edfbfa8a25ff8bf959))
* **datamodel:** starting work on data model for study. Got automapper to work for study ([884689a](https://github.com/equinor/sepes-api/commit/884689ade891648277e51076630ccc2e3b0efac8))
* **dataset-dto:** added studyId field to check for studysepecific datas ([bc41a56](https://github.com/equinor/sepes-api/commit/bc41a563f20421f511931af0b2767fce8bfce6ac))
* **dataset, datasetservice:** added StorageAccountName, and made some fields required ([e359365](https://github.com/equinor/sepes-api/commit/e35936556810cf23c8a4433c0947f5268acd4a3c))
* **dataset, studycontroller, studyservice, automapperconfig:** adding StudySpecificDatasets ([a272df4](https://github.com/equinor/sepes-api/commit/a272df41ffd0999d2a2bd8f0bed933080df7b2d4))
* **datasetcontroller, datasetservice:** made get dataset endpoint return more details ([c65505e](https://github.com/equinor/sepes-api/commit/c65505e8bb6760687c627e32757de5b64e0d1b9b))
* **datasetcontroller, studycontroller and servives:** added put method for datasets ([cebc364](https://github.com/equinor/sepes-api/commit/cebc364295f4d66d1d7f7bcb8f3684fd1a08b9d6))
* **dataset:** deny connections and make available to sandbox ([#454](https://github.com/equinor/sepes-api/issues/454)) ([844c710](https://github.com/equinor/sepes-api/commit/844c7102ac577a8135dd4a1fe49f8460ed36575a)), closes [#458](https://github.com/equinor/sepes-api/issues/458)
* **datasetservicetests, studycontroller, study:** added field resultsandlearnings in Study ([c496cb3](https://github.com/equinor/sepes-api/commit/c496cb3e8cb31612d878e113030f56436107e6ea))
* **dataset:** showing restriction based on selected datasets ([#550](https://github.com/equinor/sepes-api/issues/550)) ([223db73](https://github.com/equinor/sepes-api/commit/223db733b22450309a9d20b4089c224a9c47b8fc))
* **dblogging, sandboxworkerservice:** provides better serviceOperationDescriptions. added in db ([e513e45](https://github.com/equinor/sepes-api/commit/e513e45dbda1459a8d78d49695d39af6087c0104))
* **delete sandbox:** implemented delete sandbox. DB entires are kept, but marked as deleted ([5bde249](https://github.com/equinor/sepes-api/commit/5bde24900b05417511ba9a5ef15ca64b39193526))
* **deplazureresrefactorandtest:** fixed unit test for azure res. Some refactor. Fixed res name util ([a350da9](https://github.com/equinor/sepes-api/commit/a350da96bede9c1ea1af12132ce5dcf2b53e3a3e))
* **deploy azure resources monitoring:** implemented methods for active resources and prov state ([e80085a](https://github.com/equinor/sepes-api/commit/e80085acc4d5d135b8783e8a03c222d1ae3bc23c))
* **deployazrueresources:** progress, now creating PIP, but still err on bastion ([eb29b8b](https://github.com/equinor/sepes-api/commit/eb29b8b47ed0401cad5816a54350f989b45816ad))
* **deployazurefeatures:** renamed from CloudResource to SandboxResource ([01e72b8](https://github.com/equinor/sepes-api/commit/01e72b8f124df5171711fa362ea77f8c3fa68736))
* **deployazureres:** added technical contact for sandbox. Connected cloudres with sandbox ([7123566](https://github.com/equinor/sepes-api/commit/71235668756b5bac89fedc2bfd2c0fbeed58367d))
* **deployazureresource:** refactor, cleaning and some improvements ([a4c19e3](https://github.com/equinor/sepes-api/commit/a4c19e3ce0e4eaaa84c6a0978f0f551de3781036))
* **deployazureresources and rbac:** started work on RBAC for studies. Also dvided StudyController ([4a1980e](https://github.com/equinor/sepes-api/commit/4a1980eac5c31d55768eb66a55c41ffaf5d38eb2))
* **deployazureresources monitoring:** started work on monitoring of az resources ([facb4cf](https://github.com/equinor/sepes-api/commit/facb4cfd479e0ab745b68feae7527bd33b672eb6))
* **deployazureresources:** added variable table and service ([9e083a0](https://github.com/equinor/sepes-api/commit/9e083a0f0eefcce606b61e88b2554ff03cd49e92))
* **deployazureresources:** changed way creating of azure resources is disabled ([f2ab85f](https://github.com/equinor/sepes-api/commit/f2ab85fa14741567b58c824a89e1b9ba6ec89b6b))
* **deployazureresources:** enforced admin role requirement. Disabled create azure resources ([780fc65](https://github.com/equinor/sepes-api/commit/780fc65a2bef3994a02aa0274a23c26e132b2967))
* **deployazureresources:** implemented SandboxResourceOperationService with basic methods ([18863e7](https://github.com/equinor/sepes-api/commit/18863e707857005cf20a9fa9e6c636d93a71d8c2))
* **deployazureresources:** progress and cleaning ([fa06ee6](https://github.com/equinor/sepes-api/commit/fa06ee6cfdfd581d35c32a1c9b5eb05773873858))
* **deployazureresources:** progress and cleaning ([473f90e](https://github.com/equinor/sepes-api/commit/473f90e98c773dd204fdaf310060f213e6157965))
* **deployazureresources:** progress, now workong in bastion ([ae874fd](https://github.com/equinor/sepes-api/commit/ae874fd15464f948f270595823432afde2fe7b95))
* **deployazureresources:** some clening and refactoring. Also impl delete for all types ([6b103bb](https://github.com/equinor/sepes-api/commit/6b103bbe4ffe4636704a99192860cc7554a23d4e))
* **deployazureresources:** startet work on the services that will deploy resources to azure ([b304f9e](https://github.com/equinor/sepes-api/commit/b304f9e868e5b5a54ffb7cae24dc1ba299b05b36))
* **deployazureresources:** starting work on deploying sandboxes to azure(Network, VMs etc) ([23bd18a](https://github.com/equinor/sepes-api/commit/23bd18a1b2a425280f963e292aa513d4ae8fd99c))
* **deployazureresources:** still stuck on bastion host ([b3cb2cd](https://github.com/equinor/sepes-api/commit/b3cb2cdc469d31e26cd7ac979d837b1b70b69ff7))
* **deployazureresource:** started work on a unit test that can generate azure resources ([de9912d](https://github.com/equinor/sepes-api/commit/de9912d9bc0d1b17fb7a4d4f240206cee1daab0e))
* **developmentenv:** Cleaned out old auth code. Added appsettings.Development to gitignore ([7738487](https://github.com/equinor/sepes-api/commit/7738487340960c5c7dfd968fef8a343b924870be))
* **dockerize:** Renamed Dockerfile inside RestApi folder. It's not used as of now. Added docker-compose to gitignore and added a sample docker-compose file to doc folder ([fc5b03d](https://github.com/equinor/sepes-api/commit/fc5b03dbc70b65410f7d91410be2033e00a651d9))
* enabled norwaywest, europenorth and europewest ([#631](https://github.com/equinor/sepes-api/issues/631)) ([10eab76](https://github.com/equinor/sepes-api/commit/10eab76dcd99791061e6bf9cc5b544bfba302580))
* **errorhandlingandresponse:** starting implementing error handling and response ([795cb89](https://github.com/equinor/sepes-api/commit/795cb890ea8c2c3d53f87c05ca1aa92a1c84877c))
* **fix vendor and wbs in studyservice:** fixed Put method of study ([ff2001d](https://github.com/equinor/sepes-api/commit/ff2001d67bd5072f252b07669cb90fc2c28cc42a))
* **get studies rbac improvement:** now in accordance to the permission matrix ([80cf2db](https://github.com/equinor/sepes-api/commit/80cf2dbe51baf85379b9d7906b7d71c914411470))
* **getdatasetforstudybyid:** added endpoint for getting a specific dataset for specifica study ([d35c274](https://github.com/equinor/sepes-api/commit/d35c274d048a4dc2239518ffdd9a9e61750da555))
* **graph search:** azure config now in AzureAd section. Added mapping for graph response ([f8f1fb0](https://github.com/equinor/sepes-api/commit/f8f1fb07d8961d1989fb55d9093308809db1d22c))
* **improve sandbox response and dto:** cleaned up returned fields. Also including resources ([ab3c528](https://github.com/equinor/sepes-api/commit/ab3c5286f4182763e942ce0317cb76fa842ecf0a))
* **logosastoken:** finished logo sas token ([01e75fc](https://github.com/equinor/sepes-api/commit/01e75fc6cc3403805f4d0c0d9fc8b9650a2ec4eb))
* **models, and dtos:** added Required Fields in DB ([7c32308](https://github.com/equinor/sepes-api/commit/7c32308d7171f99eee6858e8916bcc19471cebea))
* **more rbac:** better connection between Ad and User table. Adding role on study creation ([b827072](https://github.com/equinor/sepes-api/commit/b827072b45c3e743b7c162839910884b23c4b4fb))
* **participants:** new endpoint to add users outside azure ad ([3f5d4dd](https://github.com/equinor/sepes-api/commit/3f5d4ddcabbce6d4bff47544b1721317dde8c026))
* **radix:** Renamed env variables to SEPES naming convention ([375e4aa](https://github.com/equinor/sepes-api/commit/375e4aac142e89eccd98a6a3b36b1abe6ce52c9f))
* **radix:** Updated radix config, removed empty build section ([f603643](https://github.com/equinor/sepes-api/commit/f6036434b1a106a4a2e0e36759353689b48b6a87))
* **removed norway west region:** resources should not be placed there anyway ([23490c3](https://github.com/equinor/sepes-api/commit/23490c376179b6239093f3d4e0bc30fe8af04eb5))
* **renamecloudresourcetosandboxresource:** renamed models, services and dtos ([fd74460](https://github.com/equinor/sepes-api/commit/fd74460d08c422a751e497f1f82186e32cf80002))
* **roles:** ensure study roles are set in Azure  ([#484](https://github.com/equinor/sepes-api/issues/484)) ([7c74fcd](https://github.com/equinor/sepes-api/commit/7c74fcde877ca2d91b46a22fe9e06164d5cbe1e1))
* **sandbox, study, controller and service:** "Delete Sandbox from Study" endpoint is implemented ([c0e368a](https://github.com/equinor/sepes-api/commit/c0e368af8be601d9983a2d2042d2ab53241cf013))
* **sandbox:** add link to cost analysis ([#412](https://github.com/equinor/sepes-api/issues/412)) ([2c42d3d](https://github.com/equinor/sepes-api/commit/2c42d3d399ce797427ffd97d949094aaade1be1e)), closes [#342](https://github.com/equinor/sepes-api/issues/342)
* **sandbox:** new endpoint that returns all available datasets, and if they are added to sandbox ([e8dbfee](https://github.com/equinor/sepes-api/commit/e8dbfeec888a04805443e1266d35d90f686f1266))
* **sandboxresourceservice:** can now get "deleted" resources from db + refactor: use expressions ([d921719](https://github.com/equinor/sepes-api/commit/d92171936921229608ba7a34d6282ccea4ab10c9))
* **sandboxservices:** creating sandbox will now do stuff in Azure ([b054dc8](https://github.com/equinor/sepes-api/commit/b054dc8d2e864ec0e30f860f88fb110db4acf2f6))
* **sandboxworkerservice => dowork():** method is no longer empty, however it is not done ([4f1a54e](https://github.com/equinor/sepes-api/commit/4f1a54eb591752c7c282935ef4b9c668b71d930e))
* **sastokenforlogos:** now generating sas token for logos ([8a02862](https://github.com/equinor/sepes-api/commit/8a02862b9c350778fded6eab6c2fd31e50aa41ba))
* **services that handle db:** update functions now sets Updated-field to UTC DateTime object ([6999b36](https://github.com/equinor/sepes-api/commit/6999b36f260280c3bfbc0fb8b170061b3313c040))
* **set study creator as owner:** had some dead references after last rename ([d025f3b](https://github.com/equinor/sepes-api/commit/d025f3b674666e0947359761fa71d4efda540194))
* **set study creator as owner:** when a user creates a study, he is added as owner for that study ([91a8949](https://github.com/equinor/sepes-api/commit/91a8949bd41da47830ae0f1e2cdec71d360a5a18))
* **study and sandbox:** can now add Sandbox to Study ([687a7dc](https://github.com/equinor/sepes-api/commit/687a7dc6e00efa95852f62de09c04c46718ae36f))
* **study, datasetstudy, dataset, dtos, startup, automapper:** endpoint for Add Dataset to Study ([aced067](https://github.com/equinor/sepes-api/commit/aced067d1c5ece5bfcfb7a4669fc35b92d8bfd33))
* **studyanddatasetmodel:** added DatasetController ([62c7be3](https://github.com/equinor/sepes-api/commit/62c7be323fdedfdcb7d2f4c50c4169f0b91e9a34))
* **studycontroller, studyservice:** implemented studyspecificdatasets, also handled merge from dev ([026dfb9](https://github.com/equinor/sepes-api/commit/026dfb92b84ca470ec7a6653dcbbd0cd2bc09320))
* **studylogosasgeneration:** more work on generating sas tokens. This might be it ([5462fe0](https://github.com/equinor/sepes-api/commit/5462fe04a0b1075b13795cea18bf67613a5fec6f))
* **studymodel:** added dataset and sandbox to study dto ([e222c5f](https://github.com/equinor/sepes-api/commit/e222c5f5eb08e5add96b5e6bde36055ccf5fa8ed))
* **studymodel:** added vendor and restricted ([0a0df4b](https://github.com/equinor/sepes-api/commit/0a0df4bcbaa3eb62978b7ac5c74a0c819982c250))
* **studymodelandstudyservice:** improved Study Model. Added initial tests for StudyService ([8808386](https://github.com/equinor/sepes-api/commit/88083865142f5ecc48eecae4e7722b38d8591f89))
* **studymodelvalidation:** added generic validation in a BaseService, only in use for Study ([41e999b](https://github.com/equinor/sepes-api/commit/41e999baafd35bd9980ff88395902a2ec6a9b9ad))
* **studyparticipant:** aDded model and endpoints for StudyParticipant ([f697c92](https://github.com/equinor/sepes-api/commit/f697c929daca0ecfdd5ed917ce8b02548d176a2a))
* **studyparticipant:** added model, service and endpoints for StudyParticipant ([b784692](https://github.com/equinor/sepes-api/commit/b784692d282c1d8c4be456d38fc840119bf61a79))
* **studyparticipant:** started work on study participant ([8f021d4](https://github.com/equinor/sepes-api/commit/8f021d40c2ca9ad531077514fe77b2ef5dca85fb))
* **studypermissions:** starting work on permissions for studies ([e922a3a](https://github.com/equinor/sepes-api/commit/e922a3a009f8be79a5de3e348811735fe7a8438b))
* **studyservice and logo:** can now add and get StudyLogos ([9920d21](https://github.com/equinor/sepes-api/commit/9920d210869c4737e4a271470edfdef57dd3b577))
* **studyservice, dataset:** more attributes in Dataset-model. Also better deletion ([6879a51](https://github.com/equinor/sepes-api/commit/6879a5121496bde82e0dc35f253382070ee3c457))
* **studyservice, studycontroller, azureblobstorageservice:** add and get Logo from study ([748e5df](https://github.com/equinor/sepes-api/commit/748e5df6257ddd5b6813789726e6a8a212e9d976))
* **studyservice, studycontroller:** added "Delete DatasetFromStudy"-Endpoint ([3ff529a](https://github.com/equinor/sepes-api/commit/3ff529a4d7004f9e66594e0b7268934cb1585c24))
* **studyservice.cs, studyspecificdatasetdto.cs, istudysrevice.cs, studycontroller.cs.:** buildable ([f321820](https://github.com/equinor/sepes-api/commit/f321820334e3a25d8c35359baa1694f0c8a111e0))
* **studyservice:** added "Delete Study"-endpoint ([244b84d](https://github.com/equinor/sepes-api/commit/244b84d45111e65a34b9e2d554d7326e190afb9a))
* **studyservice:** added Get Sandboxes and Get sandboxesById ([992a6b0](https://github.com/equinor/sepes-api/commit/992a6b0e914520cf38c256cc51acf14a717b516b))
* **studyservice:** removed unused endpoint ([dce52eb](https://github.com/equinor/sepes-api/commit/dce52ebc0275fc356390ca0c738b6acd592ec955))
* **studyservice:** wBS code is now required in study for sandbox creation ([5cf5e00](https://github.com/equinor/sepes-api/commit/5cf5e00f205231b5c11c29ca975bcab8801616ac))
* **study:** validate wbs on save ([#680](https://github.com/equinor/sepes-api/issues/680)) ([adf647e](https://github.com/equinor/sepes-api/commit/adf647ea9b13befab06d835594b496f8b68c0a33)), closes [#681](https://github.com/equinor/sepes-api/issues/681)
* **testazureresources:** improved DTOs for creation ([a699890](https://github.com/equinor/sepes-api/commit/a69989098f6abf188b87ed7cb17b93c34e21e0a6))
* **turned creation of azure resources back on:** also added lookup endpoints for region and templat ([6907479](https://github.com/equinor/sepes-api/commit/69074795557f534e6b568b3854da6710122a839c))
* **vm:** new endpoint for external link to virtual machine ([#422](https://github.com/equinor/sepes-api/issues/422)) ([1ae8097](https://github.com/equinor/sepes-api/commit/1ae8097dbf99200087502242a4192e3a8354826d))


### Bug Fixes

* (SandboxResourceController): methods missing "/api/" prefix. ([#436](https://github.com/equinor/sepes-api/issues/436)) ([19d3ff9](https://github.com/equinor/sepes-api/commit/19d3ff927823eea97ba24b80a30552c87a351e2c))
* add endpoint for getting sas key ([#541](https://github.com/equinor/sepes-api/issues/541)) ([10b9103](https://github.com/equinor/sepes-api/commit/10b9103f890809dda82ee36bb35676e1609d67c7))
* **add new partcipant:** changed email to emailaddress ([25661d6](https://github.com/equinor/sepes-api/commit/25661d6ef45268d05f9abe5282a746d39be3f401))
* add nullchecks for azureVmUtil and add tests ([b64cf0d](https://github.com/equinor/sepes-api/commit/b64cf0d9b2081ff02a9f402d7470b3d2c8f5904c))
* **api:** enable cors for only configured domains, used to be open for all ([#652](https://github.com/equinor/sepes-api/issues/652)) ([06bd5ea](https://github.com/equinor/sepes-api/commit/06bd5ea0031c34413f268fccb33e90b5e0f00562))
* **appi:** Fixed typo in app insight instrumentation key config ([8235d16](https://github.com/equinor/sepes-api/commit/8235d16870ee07e31fa359415b158df76c78d86c))
* **auth:** convert to PKCE auth flow ([#656](https://github.com/equinor/sepes-api/issues/656)) ([8cd0b19](https://github.com/equinor/sepes-api/commit/8cd0b1959e5c64f1e780ede969d5e13049c0e62d))
* **auth:** fixed login redirect issue. Not requiring User.Read scope on signin anymore ([1f74345](https://github.com/equinor/sepes-api/commit/1f74345f87e49744738425dd4c63be8e88121f36))
* **azure services and tests:** fixed tests and dependency injection so that AzureTest runs ([1c279af](https://github.com/equinor/sepes-api/commit/1c279af98249a01d14696077d53593352e31ab44))
* **azureblobstorageservice:** changed Azure container to studylogos ([f95e047](https://github.com/equinor/sepes-api/commit/f95e0476627f19a85410ce3861fb7422063415a2))
* **azureblobstorageservice:** using enum to validate filetypes ([d160c89](https://github.com/equinor/sepes-api/commit/d160c893ac214c3d17eb9ed80e620d3b888e19e1))
* azurekeyvaultservice logger injection ([51222c9](https://github.com/equinor/sepes-api/commit/51222c92534789362ad53b9e6364f942f10c57f4))
* **azurequeueservicetest:** was not building, commented out ([#370](https://github.com/equinor/sepes-api/issues/370)) ([10343ef](https://github.com/equinor/sepes-api/commit/10343ef9ad8237ed8fa57b167bcbca2653ada9ba))
* **azureresourcemonitoringservice:** made tag checking/updating safer ([ce78dea](https://github.com/equinor/sepes-api/commit/ce78deaf4017774417dbfd53c9dabb71b35f026b))
* **azureservice:** minor fix. Commenting ([bd352c4](https://github.com/equinor/sepes-api/commit/bd352c437aacecde787cedd17fa4b1e11c00c9cf))
* **azureservicetest:** changed region from NorwayWest to NorwayEast ([10d074d](https://github.com/equinor/sepes-api/commit/10d074da04ea8777a09dcfdf783486ee260de083))
* **basicservicecollectionfactory, studyservicetests:** fixed Dependency Injection in tests ([4e4ac00](https://github.com/equinor/sepes-api/commit/4e4ac007a9d08bb9bb60ab0a0cd1657592e9801f))
* better handle TaskCancelledException and dont log as error ([#558](https://github.com/equinor/sepes-api/issues/558)) ([01c402c](https://github.com/equinor/sepes-api/commit/01c402cd233627cde4fb9d10c3163303a2bd3c66)), closes [#483](https://github.com/equinor/sepes-api/issues/483)
* **build error in test project:** fixed error usings ([a8e1a39](https://github.com/equinor/sepes-api/commit/a8e1a39c4a06ae3fa9e828c71084faf90d3be024))
* **changelog:** contains all changes from early morning, does not look nice ([#394](https://github.com/equinor/sepes-api/issues/394)) ([ea8d1df](https://github.com/equinor/sepes-api/commit/ea8d1df73a0cc3efed7d65ed1ccc72efb41cde58))
* **config:** Fixed typo for db connection string in confg and key vaulkt ([5226dc4](https://github.com/equinor/sepes-api/commit/5226dc47546a1f0335cef5c235dbc66913d00dd4))
* **cypress:** use group access from token for mock user ([#691](https://github.com/equinor/sepes-api/issues/691)) ([7c84d67](https://github.com/equinor/sepes-api/commit/7c84d675e9c0c321c74edaa8c91a6d3482f05a01))
* **dataset:** add properties key and last modified to filelist ([#655](https://github.com/equinor/sepes-api/issues/655)) ([db73634](https://github.com/equinor/sepes-api/commit/db73634a13b2b548da2e4ca678242bb57bc34096))
* **dataset:** added permissions to study specific dataset response (c… ([#471](https://github.com/equinor/sepes-api/issues/471)) ([1fb5b4e](https://github.com/equinor/sepes-api/commit/1fb5b4e42f5fc9faf451f0bc69b254a88687f85d))
* **dataset:** assign custom role for dataset resource group ([#699](https://github.com/equinor/sepes-api/issues/699)) ([9a4d9af](https://github.com/equinor/sepes-api/commit/9a4d9af4e5c1389ac3b2b6039307cbc9fddb4c8d)), closes [#692](https://github.com/equinor/sepes-api/issues/692)
* **dataset:** better handling of missing resource group edge case ([#601](https://github.com/equinor/sepes-api/issues/601)) ([94fa316](https://github.com/equinor/sepes-api/commit/94fa31623a8904d6473c9394e061abeb1390ccd7))
* **dataset:** delete not working ([#654](https://github.com/equinor/sepes-api/issues/654)) ([32f6de9](https://github.com/equinor/sepes-api/commit/32f6de9ebea4ce7819552fb841e16c520f5b6992))
* **dataset:** ensuring container exist when getting file list ([#597](https://github.com/equinor/sepes-api/issues/597)) ([e202ea3](https://github.com/equinor/sepes-api/commit/e202ea33c588d801ae8d05e20352fe309df0ba71))
* **dataset:** ensuring rg exist on every dataset create ([d6cd6d9](https://github.com/equinor/sepes-api/commit/d6cd6d9258c5890656b8b74912c00fb2f05f3c08))
* **dataset:** getting server ip retries 3 times and caching result ([#586](https://github.com/equinor/sepes-api/issues/586)) ([ce8fd96](https://github.com/equinor/sepes-api/commit/ce8fd961be0fc7dec5333a0ded9780821f92dd4a))
* **dataset:** increased dataset max upload size ([23be629](https://github.com/equinor/sepes-api/commit/23be62957e9b50fdae780dea438dee3e6abfec35))
* **dataset:** increased upload sas token timeout to 30m ([64360c4](https://github.com/equinor/sepes-api/commit/64360c4a5b9b1059e92b5b08889339bb752dbc95))
* **datasetmodel:** changed studyno to studyid ([aad03bd](https://github.com/equinor/sepes-api/commit/aad03bde9a139751bc545f7fcd2f0f361f9d61ad))
* **dataset:** new sas token endpoing for deleting files ([#592](https://github.com/equinor/sepes-api/issues/592)) ([b818d10](https://github.com/equinor/sepes-api/commit/b818d1082b1ac00015e9a2c24edb4e0f9e7ec646)), closes [#590](https://github.com/equinor/sepes-api/issues/590)
* **dataset:** now adding correct client ip firewall rule ([ce3c464](https://github.com/equinor/sepes-api/commit/ce3c464dea5bb86093daf51cdef9986f06f2fcff))
* **dataset:** now checking study specific access when reading dataset ([#533](https://github.com/equinor/sepes-api/issues/533)) ([06f40fc](https://github.com/equinor/sepes-api/commit/06f40fcea5fe8aa11e27f269e57159420f988d67)), closes [#532](https://github.com/equinor/sepes-api/issues/532)
* **dataset:** now passing correct arguments to role assignment service ([#492](https://github.com/equinor/sepes-api/issues/492)) ([e27ab4c](https://github.com/equinor/sepes-api/commit/e27ab4c13d7e6e49d32859a3856cdc407820c974))
* **datasetservice, datasetcontroller:** studySpecificDatasets not available from datasetcontroller ([6e0ce98](https://github.com/equinor/sepes-api/commit/6e0ce98b49235d64b02b089a8da94e3bcbf68c49))
* **datasetservice:** added check to see if StorageAccountName is supplied ([bcbb9a4](https://github.com/equinor/sepes-api/commit/bcbb9a4812ec38f557bd16c79525eea71f9af12b))
* **dataset:** set azure role assignments for storage account and resource group ([#513](https://github.com/equinor/sepes-api/issues/513)) ([0949136](https://github.com/equinor/sepes-api/commit/09491361e13b9646355cefda0b4d3bab163cedc0))
* **datasetsrevice:** forgot to register datasetservice ([f3c80fb](https://github.com/equinor/sepes-api/commit/f3c80fb6b69ad11bad47c99b3bf315dfe385c4b1))
* **DatasetUtils:** add nullcheck ([#618](https://github.com/equinor/sepes-api/issues/618)) ([1e64848](https://github.com/equinor/sepes-api/commit/1e6484821d9c17438fe41ed1b96a69cb6273002d))
* **deployazureresourcesdidnotbuild:** forgot to comment out some temp code tpo make it build ([dddf886](https://github.com/equinor/sepes-api/commit/dddf886a70128264e3998a463513ccaa9ef84455))
* **docker:** Changed expose port to 80 instead of 5001. Modified docker-compose after splitting repos. ([cb43f8e](https://github.com/equinor/sepes-api/commit/cb43f8e1d6f288b4ee8dbea0db1e3c1474e0bb9d))
* ensured relevant tags are being set for all resources ([309eba4](https://github.com/equinor/sepes-api/commit/309eba4c65d624b5f44156e268bd026242a11c6f))
* filter out dash in storage account name ([#736](https://github.com/equinor/sepes-api/issues/736)) ([63592a0](https://github.com/equinor/sepes-api/commit/63592a08b2b13a363dbb59b9f585d7c65bb36943))
* **fixed argument to method with wrong name:** error occured when merging, sending wrong arg name ([e117e96](https://github.com/equinor/sepes-api/commit/e117e96709bbe89212ab3bb2678e48bb078e738c))
* **function:** run as managed identity ([#700](https://github.com/equinor/sepes-api/issues/700)) ([3b50e56](https://github.com/equinor/sepes-api/commit/3b50e56161a181ae63ae3df45dc66045cb05ed7a)), closes [#669](https://github.com/equinor/sepes-api/issues/669)
* **function:** updated managed by ids for dev and prod ([#663](https://github.com/equinor/sepes-api/issues/663)) ([03d44d6](https://github.com/equinor/sepes-api/commit/03d44d6f5dfbc51507b454dd292de166f49fc01c))
* **IHasLogoUrl:** Removed public in interface ([d974a00](https://github.com/equinor/sepes-api/commit/d974a00b5694b59efbba01d22711eedf9a661fc8))
* issue where space was not allowed in study, sandbox and vm name ([f77ccb8](https://github.com/equinor/sepes-api/commit/f77ccb85e37d041b53ca01aed880dfab5981f3e7))
* **keyvault:** clean old passwords, changed created filter to use utc ([a31feb8](https://github.com/equinor/sepes-api/commit/a31feb8d371d4d3432be885aa11e95058ff80e56))
* **logging:** clean up appi event ids ([#670](https://github.com/equinor/sepes-api/issues/670)) ([869ccca](https://github.com/equinor/sepes-api/commit/869ccca26774791556506cae752d37740ba21cb6))
* **logging:** dont log all provisioning exceptions as exceptions to App Insights ([#498](https://github.com/equinor/sepes-api/issues/498)) ([037e4c7](https://github.com/equinor/sepes-api/commit/037e4c7fd530adeaf82c7631efeb36f0139b886f))
* **logging:** Fixed typo in StudyController ([7430a0a](https://github.com/equinor/sepes-api/commit/7430a0a0d4a6865be827c93dc5c170dbc8440dd6))
* **make it build:** project did not build on last attempt ([c9d7336](https://github.com/equinor/sepes-api/commit/c9d7336e4144bdaf2ddfcf1a75702137879ac9bc))
* **minor:** changed extension for radixconfig ([a6d635c](https://github.com/equinor/sepes-api/commit/a6d635c76a454afaefcff5e8ad796b46e01dc05d))
* **monitoring:** improved log messages, added standardized eventids ([#580](https://github.com/equinor/sepes-api/issues/580)) ([37cf7f4](https://github.com/equinor/sepes-api/commit/37cf7f4515e6656ea871c68fba464f03a293750a))
* New endpoint for study roles where it will return which role a u… ([#560](https://github.com/equinor/sepes-api/issues/560)) ([673283b](https://github.com/equinor/sepes-api/commit/673283b7980f6a60e0dbdbc93faae2cdfdeb1e3a))
* **partcipants endpoint:** changed back to /api on endpoints ([bc68132](https://github.com/equinor/sepes-api/commit/bc6813243aa61c5bc9ce3e46670d74aac1d8682e))
* participant search from employees and affiliates ([#701](https://github.com/equinor/sepes-api/issues/701)) ([35f8a80](https://github.com/equinor/sepes-api/commit/35f8a80b82d2afc74a436a2338a140976414bd44))
* **participant:** added fields that were missing. Role is still missing ([1ce426e](https://github.com/equinor/sepes-api/commit/1ce426ea7eb91aaa008fcc89fc973e8b597a6b2f))
* **participantdtomissingfield:** participatDto was missing UserName and EmailAddress ([14969fe](https://github.com/equinor/sepes-api/commit/14969fe63821f4bbfd327a54c8f47f615f031927))
* **participant:** fixed error in participant lookup where same role was added multiple times ([#567](https://github.com/equinor/sepes-api/issues/567)) ([c8d79c9](https://github.com/equinor/sepes-api/commit/c8d79c99b2970e80b17e1b544e28247b932ae911))
* **participant:** remove created by filter on existing role assignments before add ([#667](https://github.com/equinor/sepes-api/issues/667)) ([47c8663](https://github.com/equinor/sepes-api/commit/47c8663c6add37b6f2b6a2399f59a70af8c49888))
* **participants:** issue where sepes was dependent on Azure to return… ([#502](https://github.com/equinor/sepes-api/issues/502)) ([f34f980](https://github.com/equinor/sepes-api/commit/f34f980bcd635de0832ab4f2b0ef42dfda7e7448)), closes [#499](https://github.com/equinor/sepes-api/issues/499)
* prevent fail in resource naming  ([#709](https://github.com/equinor/sepes-api/issues/709)) ([7345f92](https://github.com/equinor/sepes-api/commit/7345f9253db34eee3a8beb670191f6ec94b2cb21)), closes [#707](https://github.com/equinor/sepes-api/issues/707)
* prevent timeout when getting studies ([#645](https://github.com/equinor/sepes-api/issues/645)) ([44e329b](https://github.com/equinor/sepes-api/commit/44e329b13348fd87ec306792f4199c3f4444d24f))
* **radix:** Added logging for usehttpsredirection ([05aae97](https://github.com/equinor/sepes-api/commit/05aae97e50e4f6558d79480e7def93fba2c82af6))
* **radixconfig:** Updated APPI key in radixconfig ([41f84c6](https://github.com/equinor/sepes-api/commit/41f84c6cc2578d129a6c45c13aebba9f5eb8bc35))
* **radix:** Moved enivronment variables to backend secton, ahd palced them in front end ([154535b](https://github.com/equinor/sepes-api/commit/154535baf91cf0bff952c1b3e65586f4fdd508eb))
* **radix:** Playing with ports in radixconfig ([2386b60](https://github.com/equinor/sepes-api/commit/2386b60f4802057fd4080fdb906c5d3a6a44679c))
* **radix:** Set httonly to true in radixconfig ([99f3418](https://github.com/equinor/sepes-api/commit/99f3418817f71d6696980d7fe430e6bbbbfe52dc))
* **radix:** Update radixconfig with new application name ([2d32bd6](https://github.com/equinor/sepes-api/commit/2d32bd6dabf25d4b29bd79b67048810818257120))
* **rbac:** added missing permissions for admin ([#445](https://github.com/equinor/sepes-api/issues/445)) ([ea8a189](https://github.com/equinor/sepes-api/commit/ea8a189d4b8ee9a576ccefc685227bfac9e42a5b))
* **rbac:** added role for employee vs external user. more unit tests.… ([#440](https://github.com/equinor/sepes-api/issues/440)) ([12f44a6](https://github.com/equinor/sepes-api/commit/12f44a6c0f4ab97917f7c0eb9bd69ce3eb9e044f))
* **rbac:** admin was still missing some permissions that only he shold have ([37087f1](https://github.com/equinor/sepes-api/commit/37087f1f1beffccd05739386995acd5ef1080913))
* **rbac:** now allowing b2c user access ([#531](https://github.com/equinor/sepes-api/issues/531)) ([ac61458](https://github.com/equinor/sepes-api/commit/ac61458549a557960b9e7f83c08391adb8b6efb5))
* reduce max length of description ([3bc80bc](https://github.com/equinor/sepes-api/commit/3bc80bc41b5a64874291a20a4e0fff9da147d797))
* **remove participant:** fixed lambda expression ([141f3c1](https://github.com/equinor/sepes-api/commit/141f3c1e74ec45f0413d0144fad0924bc33daa28))
* renamed dataset properties in permissions response. Also now using rbac engine ([#432](https://github.com/equinor/sepes-api/issues/432)) ([895b713](https://github.com/equinor/sepes-api/commit/895b713f5607c68dfaca133a2b4ca3bb93cf393b))
* Require names of studies, vms and sandboxes to have 3 or more ch… ([#509](https://github.com/equinor/sepes-api/issues/509)) ([bd169eb](https://github.com/equinor/sepes-api/commit/bd169eb95e37aba0f428ddc9c7c2c6857d6dc498)), closes [#495](https://github.com/equinor/sepes-api/issues/495)
* **resourcegroupnaming:** ensured that naming of resourceGroups are consistent and pseudo-unique ([511c578](https://github.com/equinor/sepes-api/commit/511c578c49ee30a9764f4d18e8002fd9e02a3d0a))
* **sandbox creation:** ensuring study and sandbox name part of resour… ([#401](https://github.com/equinor/sepes-api/issues/401)) ([fe26189](https://github.com/equinor/sepes-api/commit/fe26189c902994850f36bcd0dbf4198a3a4f521d))
* **sandbox creation:** rule priority for basic nsg rule was to high, now set to 4050 ([946009f](https://github.com/equinor/sepes-api/commit/946009fbe688366f5058df1270331eec6e1e6290))
* **sandbox resource list:** fixed when status is showing create failed for deleted resources ([0766619](https://github.com/equinor/sepes-api/commit/07666193158f18f18a2373d1455f71b47595b1bc))
* **sandbox:** add correct dataset restrictions text ([3b3afd8](https://github.com/equinor/sepes-api/commit/3b3afd839d85af59949cd9c4ae59eb1829e66c68))
* **sandbox:** add/remove dataset. Improved validation and messages. Now returning HTTP 204 on succes ([d6f534c](https://github.com/equinor/sepes-api/commit/d6f534cbce43ff1794b81605c3e87cf343abf8dc))
* **sandbox:** added null checks to resource endpoint ([607d8db](https://github.com/equinor/sepes-api/commit/607d8dba15105ae5d7125df00f69e025f6321d03))
* **sandbox:** attempt to prevent fail of role assignment update task ([#649](https://github.com/equinor/sepes-api/issues/649)) ([fe12b58](https://github.com/equinor/sepes-api/commit/fe12b58e70bb7e7d6246303139d779c33b578a73))
* **sandbox:** better null handling and logging in vnet creation ([#610](https://github.com/equinor/sepes-api/issues/610)) ([bbc7238](https://github.com/equinor/sepes-api/commit/bbc723859b8bb19dfcb903397720ce1dd5b26699))
* **sandbox:** changed when resource retry link is created ([#600](https://github.com/equinor/sepes-api/issues/600)) ([baec250](https://github.com/equinor/sepes-api/commit/baec250c264a77894b51da3ac1d2cae228122281))
* **sandbox:** fixed mapping error occuring in sandbox response. ([#582](https://github.com/equinor/sepes-api/issues/582)) ([e01224f](https://github.com/equinor/sepes-api/commit/e01224f23004596deef11978ab9bec36277dc224))
* **sandbox:** fixed mapping in resource response ([#568](https://github.com/equinor/sepes-api/issues/568)) ([a8673a4](https://github.com/equinor/sepes-api/commit/a8673a4937b668c41d6a591b9c8fb82210094787))
* **sandbox:** Include disk price to the estimated cost of an vm ([#469](https://github.com/equinor/sepes-api/issues/469)) ([cd06148](https://github.com/equinor/sepes-api/commit/cd06148be05760c7a6706c4d9d2dde1252d7aaf3))
* **sandbox:** new endpoint for getting cost analysis ([1f8d837](https://github.com/equinor/sepes-api/commit/1f8d8374dd41b3f4b76c4592ede730a760c2d74a))
* **sandbox:** removed duplicate check of existing name ([71a2abc](https://github.com/equinor/sepes-api/commit/71a2abc5b28ab3671ff5ffb3c00bc00884a1116f))
* **sandboxservice:** sandboxService now throws NotFoundException if sandbox is not found ([cb956ae](https://github.com/equinor/sepes-api/commit/cb956aefc75d92902fb4c653b3aea6617e74cb54))
* **sandbox:** sort sizes by price with lowest cost first ([#462](https://github.com/equinor/sepes-api/issues/462)) ([f56f01d](https://github.com/equinor/sepes-api/commit/f56f01d9402cc78d257452e9a4aea9e811a17b50)), closes [#423](https://github.com/equinor/sepes-api/issues/423)
* **sandbox:** timeout when going to next phase ([#621](https://github.com/equinor/sepes-api/issues/621)) ([b4b901a](https://github.com/equinor/sepes-api/commit/b4b901abeac65c2c089f99566c338caaded61ef1))
* **sepesdbcontext:** now adds datetime to Created and Updated field on Resource and Operations ([3ca8bf7](https://github.com/equinor/sepes-api/commit/3ca8bf7820153b9cc8bdb610b31dcccefa6ca970))
* standardized how azure services handles non existing resources ([#599](https://github.com/equinor/sepes-api/issues/599)) ([13d565f](https://github.com/equinor/sepes-api/commit/13d565f4c8eb204ef6b656a43d6c37253c014113))
* **startup.cs:** changed JSON formatting back to camelCase ([6a7d3cf](https://github.com/equinor/sepes-api/commit/6a7d3cfcff8ee19839365b6736665a196ee44ffe))
* **startup:** better logging for empty db con string ([d40427f](https://github.com/equinor/sepes-api/commit/d40427f48a5337d24ec74f692a0ca7c914a65ee0))
* **startup:** Renamed Appi key with SEPES_ prefix. Added some logg ing to program.cs and Starup.cs ([550f259](https://github.com/equinor/sepes-api/commit/550f259650776142fdcbd1940b5826f3bc01480d))
* **studies:** external users not  seeing all studies in list anymore ([#537](https://github.com/equinor/sepes-api/issues/537)) ([3bd15bd](https://github.com/equinor/sepes-api/commit/3bd15bd09f60173aecaf9f2b0b41ca63992f207e))
* **study create:** was failing due to missing permission check ([#376](https://github.com/equinor/sepes-api/issues/376)) ([7a01d95](https://github.com/equinor/sepes-api/commit/7a01d95b3d8cc05a834859d5b243439c961cd645))
* **study details:** missing include causing mapping error for StudyDatasets > SandboxDataset ([#414](https://github.com/equinor/sepes-api/issues/414)) ([9ae327c](https://github.com/equinor/sepes-api/commit/9ae327c3ccfb1452e32928b2b7d9a09a45003dc1))
* **study- service and controller, startup:** completed merge, and fixed errors ([f2b5ce2](https://github.com/equinor/sepes-api/commit/f2b5ce2900a8e533110b55efc231677b09302813))
* **study:** add and remove participant did not save ([a78d8e4](https://github.com/equinor/sepes-api/commit/a78d8e47abded38395a065bc6a60ebf831317271))
* **studycontroller:** added FromForm to fix headers ([4860e09](https://github.com/equinor/sepes-api/commit/4860e09c0de27868e828027401ffcf4b83554ee2))
* **studycontroller:** added missing "/" in Put method for changing datasets ([b26842e](https://github.com/equinor/sepes-api/commit/b26842e7241c952243aa6630265799af519fb0db))
* **study:** delete failed when no dataset resource group was set ([#444](https://github.com/equinor/sepes-api/issues/444)) ([ff30438](https://github.com/equinor/sepes-api/commit/ff30438fa862549897018be534b729462e7078d7))
* **study:** delete sometimes failed when no resource group created ([#525](https://github.com/equinor/sepes-api/issues/525)) ([558c483](https://github.com/equinor/sepes-api/commit/558c483028108426613a45686cbd064a8e8dd5e9))
* **study:** details response also including datasets not in any sandbox ([#695](https://github.com/equinor/sepes-api/issues/695)) ([c3a972a](https://github.com/equinor/sepes-api/commit/c3a972a7756e8e5eab1f3bb0072463ac10daf6af)), closes [#694](https://github.com/equinor/sepes-api/issues/694)
* **studyDetails:** added which sandboxes a given datasets is used in ([#403](https://github.com/equinor/sepes-api/issues/403)) ([d960358](https://github.com/equinor/sepes-api/commit/d960358bab05d8cc9b3a2c02281698375c831adb))
* **studydtohassasforlogoevenwhennologo:** study dto got sas token even when there was no logo url ([86591a1](https://github.com/equinor/sepes-api/commit/86591a1306e5189d2c8ea1d06f478296c948e80d))
* **studydtologourlsas:** studyDto did not get sas for logo url. Now fixed ([53bcb56](https://github.com/equinor/sepes-api/commit/53bcb56e5a9cddec4bdbc67d38661d131fc2eed3))
* **study:** fix slow study close due to include ([#668](https://github.com/equinor/sepes-api/issues/668)) ([e1628fd](https://github.com/equinor/sepes-api/commit/e1628fd1a738c26d352da41e2ee7f496b1208929))
* **study:** increase logo sas token timeout to 61 min ([#726](https://github.com/equinor/sepes-api/issues/726)) ([00c5a6f](https://github.com/equinor/sepes-api/commit/00c5a6f19e266ddafc700143c0d339829c3a5b72))
* **study:** increase performance for GET resultsandlearnings ([#607](https://github.com/equinor/sepes-api/issues/607)) ([8bdedc5](https://github.com/equinor/sepes-api/commit/8bdedc5d6ef520124cb0e48c62046d9c25de5d10))
* **study:** issue with role list and admin role ([#716](https://github.com/equinor/sepes-api/issues/716)) ([15bcc16](https://github.com/equinor/sepes-api/commit/15bcc163dddb225782ca6ea9a7a9e4d4dc6cbc1e)), closes [#710](https://github.com/equinor/sepes-api/issues/710)
* **studymodel:** vendor and WBS did not get saved ([6907801](https://github.com/equinor/sepes-api/commit/6907801ce36fd31d4065f30cd65504245dee81b9))
* **study:** now returning lighter response for study details endpoint ([15c4f02](https://github.com/equinor/sepes-api/commit/15c4f026938f0f563e7b70164d51abd283dc5593))
* **study:** now sponsor and sponsor rep are allowed to soft delete st… ([#653](https://github.com/equinor/sepes-api/issues/653)) ([1801f72](https://github.com/equinor/sepes-api/commit/1801f72cc9dbce3fd0c5ed6761c39589d0f2f986))
* **study:** participant search ([#723](https://github.com/equinor/sepes-api/issues/723)) ([66e0267](https://github.com/equinor/sepes-api/commit/66e0267298658966203b6a179d2fa6cc46f95a6c)), closes [#712](https://github.com/equinor/sepes-api/issues/712)
* **studyparticipant:** added back id of participant ([64d6526](https://github.com/equinor/sepes-api/commit/64d65264d239f0eb2319725c3c75e2e27bf98d2a))
* **studyparticipantwrongdto:** adde correct fields to studyparticipant ([6416f2c](https://github.com/equinor/sepes-api/commit/6416f2c70bfa6fb47024b34b183c8236b2feed7f))
* **study:** prevent setting invalid wbs if active sandbox or dataset ([#705](https://github.com/equinor/sepes-api/issues/705)) ([f909a21](https://github.com/equinor/sepes-api/commit/f909a211f6cdc97a388337245c44370834818491)), closes [#703](https://github.com/equinor/sepes-api/issues/703) [#704](https://github.com/equinor/sepes-api/issues/704)
* **study:** prevented details request deadlock ([#611](https://github.com/equinor/sepes-api/issues/611)) ([d2c0859](https://github.com/equinor/sepes-api/commit/d2c085977f8110135995befcecd4ac50384de629))
* **study:** remove logo if logoUrl is empty ([#589](https://github.com/equinor/sepes-api/issues/589)) ([60aa506](https://github.com/equinor/sepes-api/commit/60aa506d0c306a54b4ae14a144197644ee2d2e4c)), closes [#588](https://github.com/equinor/sepes-api/issues/588)
* **study:** returning the detailed response for CreateStudy, UpdateLogo and UpdateStudyDetails ([f5a4fb1](https://github.com/equinor/sepes-api/commit/f5a4fb1fef220032e5f6a8f83147e029d4bd93ea))
* **studyservice, studycontroller:** moved _config from controller to service ([9c69fbe](https://github.com/equinor/sepes-api/commit/9c69fbe8a7a56882bb2200a3dd5a394b5b6e34b8))
* **studyservice.cs:** when deleting study, method will now delete studylogo from Azure blob Storage ([94ffb5e](https://github.com/equinor/sepes-api/commit/94ffb5eab8d6ee60887a0c7988507a1fba03ae66))
* **studyservice:** updateStudyDetails now updates ResultsAndLearnings field ([af1bafd](https://github.com/equinor/sepes-api/commit/af1bafdf2df768feb165d62a3882f85371501bca))
* **study:** streamlined create and add logo ([#603](https://github.com/equinor/sepes-api/issues/603)) ([77efc2b](https://github.com/equinor/sepes-api/commit/77efc2bb02e7181f3cb779eefb16b386c1790e3c))
* **study:** wbs cache use dapper and isolate errors ([#724](https://github.com/equinor/sepes-api/issues/724)) ([ae49bfa](https://github.com/equinor/sepes-api/commit/ae49bfadd0e5ef278a88827e7db3b10df99f63c1)), closes [#720](https://github.com/equinor/sepes-api/issues/720)
* **stuyresourcegroupnameandtests:** fixed resource group name for studies. Some cleaning ([1f9da6d](https://github.com/equinor/sepes-api/commit/1f9da6d5ecc15500466c7ebeb460b110900c3be1))
* **technicalcontact info was missing on sandbox:** getting it from claimsprincipal ([8e12841](https://github.com/equinor/sepes-api/commit/8e1284104fcb63082374a4c15dc2e1281c34e417))
* **user util  unable to find username:** now added relevant scope on frontend. Can clean up here ([536e023](https://github.com/equinor/sepes-api/commit/536e023ee98d9b43f90e5e506de018a05d054dcc))
* Validate firewall IP rules ([1866644](https://github.com/equinor/sepes-api/commit/18666447832bbb0c0d8fcd514af345a9448a8349))
* **virtual machine:** resolve empty size lookup for some regions ([#402](https://github.com/equinor/sepes-api/issues/402)) ([b628869](https://github.com/equinor/sepes-api/commit/b628869538e1d61e883ce9caef43fbfb09f24865))
* **vm rules:** fixed issue where vm creation and update lost got off track ([#416](https://github.com/equinor/sepes-api/issues/416)) ([a8025b0](https://github.com/equinor/sepes-api/commit/a8025b08abd6ea77d2a542207aa5e9ca22e0c27e))
* **vm rules:** now keeping track of priorities, asking azure when creating rules ([#413](https://github.com/equinor/sepes-api/issues/413)) ([485ad82](https://github.com/equinor/sepes-api/commit/485ad82b8500dd5c1bba3d6621705687b992f9bc))
* **vm rules:** returning empty list instead of null, when no rules exist ([#379](https://github.com/equinor/sepes-api/issues/379)) ([7bf027d](https://github.com/equinor/sepes-api/commit/7bf027d37a97c64408108e6dbcd3ee33446d8de8))
* **vm:** add endpoint for validating a username. Some names and perio… ([#479](https://github.com/equinor/sepes-api/issues/479)) ([6394c24](https://github.com/equinor/sepes-api/commit/6394c24471930f99d8c4ca278f017a24f3f30962)), closes [#478](https://github.com/equinor/sepes-api/issues/478)
* **vm:** filtered away zrs disks ([#634](https://github.com/equinor/sepes-api/issues/634)) ([08e3ca3](https://github.com/equinor/sepes-api/commit/08e3ca331b08ba0da503bf0e00f6f5fd1fd69a10))
* **vm:** improved selection of price for all SKUs. Avoiding "low priority" and "spot" if possible ([acec3d4](https://github.com/equinor/sepes-api/commit/acec3d4099e7ed73ce2136c0a722da5cc0957df6))
* **vm:** issue with password validation: Updated test ([fc0577f](https://github.com/equinor/sepes-api/commit/fc0577f31478599862dc02b6ef4916e0cd63767f)), closes [#504](https://github.com/equinor/sepes-api/issues/504) [#503](https://github.com/equinor/sepes-api/issues/503)
* **vm:** new endpoint for getting price of an vm without calling azure ([43aa823](https://github.com/equinor/sepes-api/commit/43aa8232ce84385f9b3efeab3d84061dd59e2c87))
* **vm:** no public IP was created for VM ([#696](https://github.com/equinor/sepes-api/issues/696)) ([11f84e8](https://github.com/equinor/sepes-api/commit/11f84e89a2f4014f147715dc7ac184f1355f9d97)), closes [#693](https://github.com/equinor/sepes-api/issues/693)
* **vm:** now disables disk cache from 4tb ([#632](https://github.com/equinor/sepes-api/issues/632)) ([9047a14](https://github.com/equinor/sepes-api/commit/9047a140d4bedeb029dcc1f0a8d6ab72e9fcea1f))
* **vm:** now rolling back, if creation fails in the create response ([#490](https://github.com/equinor/sepes-api/issues/490)) ([91b87f9](https://github.com/equinor/sepes-api/commit/91b87f9bf33d042a1fa5d29673eb2a5f73e447b8))
* **vm:** potential fix for failed creation in other users sandbox ([ade6540](https://github.com/equinor/sepes-api/commit/ade6540d82081530a53d4772ff15b476e2fb625a))
* **vm:** provisioning can now resume if last attempt failed ([#491](https://github.com/equinor/sepes-api/issues/491)) ([084e030](https://github.com/equinor/sepes-api/commit/084e0300c02e320478ddde3781f98b48d151b029))
* **vm:** username validation now takes what os is picked into consideration ([2ff065d](https://github.com/equinor/sepes-api/commit/2ff065d6f6caa49dc4ac5c5c14dd60d0af5a3c46)), closes [#521](https://github.com/equinor/sepes-api/issues/521)
* **vm:** validate password and throw error if it does not meet requir… ([#476](https://github.com/equinor/sepes-api/issues/476)) ([2b1cd7b](https://github.com/equinor/sepes-api/commit/2b1cd7be5738676b5df138c04832a0bf02495192)), closes [#337](https://github.com/equinor/sepes-api/issues/337)
* **vm:** vm extended info endpoint, renamed MemoryInDb to MemoryGB ([#424](https://github.com/equinor/sepes-api/issues/424)) ([939cca7](https://github.com/equinor/sepes-api/commit/939cca7d7520f74defca8edd3feaac245a85fd31))
* **vm:** vm name validation can now handle special characters. For instance question mark ([#482](https://github.com/equinor/sepes-api/issues/482)) ([050f53a](https://github.com/equinor/sepes-api/commit/050f53af5cc20ac4f812464f4484edefb28da141)), closes [#481](https://github.com/equinor/sepes-api/issues/481)
* **vm:** vm not getting stuck in updating if previous attempt failed ([#470](https://github.com/equinor/sepes-api/issues/470)) ([b9e5685](https://github.com/equinor/sepes-api/commit/b9e568540e36e94ac8e0326a29bba2d5717cac16))
* **wbsvalidation:** better cache duplication handling ([#718](https://github.com/equinor/sepes-api/issues/718)) ([f32b3ff](https://github.com/equinor/sepes-api/commit/f32b3ffec4fecdea29bd0a6306ca459255f57fce)), closes [#717](https://github.com/equinor/sepes-api/issues/717)
* **worker:** add error to resource ([#516](https://github.com/equinor/sepes-api/issues/516)) ([80f98da](https://github.com/equinor/sepes-api/commit/80f98da82da898c2f4b541d169a955143530ea9c))
* **worker:** added trace logging in startup ([#620](https://github.com/equinor/sepes-api/issues/620)) ([7c86c3b](https://github.com/equinor/sepes-api/commit/7c86c3bbf3402518c78589f8bca2306c74a5269e))
* **worker:** decreased time taken for VM creation to start ([#463](https://github.com/equinor/sepes-api/issues/463)) ([f0fe1f7](https://github.com/equinor/sepes-api/commit/f0fe1f74fef8c320d871f3a203aa060891d52850))
* **worker:** lifespan of health service now transient ([#722](https://github.com/equinor/sepes-api/issues/722)) ([091db37](https://github.com/equinor/sepes-api/commit/091db3773dc66d17593b2ce68c537a3a09c54f36))
* **worker:** sandbox creation queue timeout and better in prog protection ([#613](https://github.com/equinor/sepes-api/issues/613)) ([1a0500e](https://github.com/equinor/sepes-api/commit/1a0500e06df9ba71358a0995b6ef44c8c8cabe00))


* feat(studyspecificdataset)/implement file upload (#443) ([6dcd4c4](https://github.com/equinor/sepes-api/commit/6dcd4c4663978a94f41a3739f81c29677dcf7227)), closes [#443](https://github.com/equinor/sepes-api/issues/443)
* Merge from develop to master (#364) ([34aa71f](https://github.com/equinor/sepes-api/commit/34aa71fe688e57c33fb3d6807029b76426c0a5e9)), closes [#364](https://github.com/equinor/sepes-api/issues/364) [#267](https://github.com/equinor/sepes-api/issues/267) [#272](https://github.com/equinor/sepes-api/issues/272) [#271](https://github.com/equinor/sepes-api/issues/271) [#274](https://github.com/equinor/sepes-api/issues/274) [#275](https://github.com/equinor/sepes-api/issues/275) [#276](https://github.com/equinor/sepes-api/issues/276) [#277](https://github.com/equinor/sepes-api/issues/277) [#279](https://github.com/equinor/sepes-api/issues/279) [#280](https://github.com/equinor/sepes-api/issues/280) [#281](https://github.com/equinor/sepes-api/issues/281) [#282](https://github.com/equinor/sepes-api/issues/282) [#285](https://github.com/equinor/sepes-api/issues/285) [#287](https://github.com/equinor/sepes-api/issues/287) [#295](https://github.com/equinor/sepes-api/issues/295) [#294](https://github.com/equinor/sepes-api/issues/294) [#297](https://github.com/equinor/sepes-api/issues/297) [#296](https://github.com/equinor/sepes-api/issues/296) [#298](https://github.com/equinor/sepes-api/issues/298) [#293](https://github.com/equinor/sepes-api/issues/293) [#299](https://github.com/equinor/sepes-api/issues/299) [#302](https://github.com/equinor/sepes-api/issues/302) [#301](https://github.com/equinor/sepes-api/issues/301) [#304](https://github.com/equinor/sepes-api/issues/304) [#306](https://github.com/equinor/sepes-api/issues/306) [#308](https://github.com/equinor/sepes-api/issues/308) [#307](https://github.com/equinor/sepes-api/issues/307) [#319](https://github.com/equinor/sepes-api/issues/319) [#317](https://github.com/equinor/sepes-api/issues/317) [#318](https://github.com/equinor/sepes-api/issues/318) [#321](https://github.com/equinor/sepes-api/issues/321) [#322](https://github.com/equinor/sepes-api/issues/322) [#323](https://github.com/equinor/sepes-api/issues/323) [#313](https://github.com/equinor/sepes-api/issues/313) [#328](https://github.com/equinor/sepes-api/issues/328) [#310](https://github.com/equinor/sepes-api/issues/310) [#330](https://github.com/equinor/sepes-api/issues/330) [#332](https://github.com/equinor/sepes-api/issues/332) [#333](https://github.com/equinor/sepes-api/issues/333) [#335](https://github.com/equinor/sepes-api/issues/335) [#338](https://github.com/equinor/sepes-api/issues/338) [#339](https://github.com/equinor/sepes-api/issues/339) [#340](https://github.com/equinor/sepes-api/issues/340) [#344](https://github.com/equinor/sepes-api/issues/344) [#341](https://github.com/equinor/sepes-api/issues/341) [#348](https://github.com/equinor/sepes-api/issues/348) [#352](https://github.com/equinor/sepes-api/issues/352) [#353](https://github.com/equinor/sepes-api/issues/353) [#355](https://github.com/equinor/sepes-api/issues/355) [#356](https://github.com/equinor/sepes-api/issues/356) [#358](https://github.com/equinor/sepes-api/issues/358) [#362](https://github.com/equinor/sepes-api/issues/362) [#361](https://github.com/equinor/sepes-api/issues/361) [#309](https://github.com/equinor/sepes-api/issues/309) [#363](https://github.com/equinor/sepes-api/issues/363)
* **deployazureresources:** cleaning up some names, unused usings and comments ([92b0610](https://github.com/equinor/sepes-api/commit/92b06102c49d06236a0793d8b177c2b6cbc11235))
* improve study read rbac coverage ([#714](https://github.com/equinor/sepes-api/issues/714)) ([b031107](https://github.com/equinor/sepes-api/commit/b031107d8b7ed3c0b9fa524f2b625e14ae19ee0f)), closes [#713](https://github.com/equinor/sepes-api/issues/713)
* removed sandbox endpoints that were simplified to not include/study/studyId part ([#437](https://github.com/equinor/sepes-api/issues/437)) ([cde78c0](https://github.com/equinor/sepes-api/commit/cde78c0d46cfcea20467f3237dad6342e620030a))
* results and learnings and study specific datasets ([#746](https://github.com/equinor/sepes-api/issues/746)) ([ccd6da9](https://github.com/equinor/sepes-api/commit/ccd6da9852b44263401b4ed6688d706717cf0f68)), closes [#742](https://github.com/equinor/sepes-api/issues/742) [#744](https://github.com/equinor/sepes-api/issues/744)

### [0.6.6](https://github.com/equinor/sepes-api/compare/0.6.5...0.6.6) (2021-08-24)

### [0.6.5](https://github.com/equinor/sepes-api/compare/0.6.4...0.6.5) (2021-08-24)

### [0.6.4](https://github.com/equinor/sepes-api/compare/0.6.3...0.6.4) (2021-08-23)

### [0.6.3](https://github.com/equinor/sepes-api/compare/0.6.2...0.6.3) (2021-08-23)

### [0.6.2](https://github.com/equinor/sepes-api/compare/0.6.1...0.6.2) (2021-08-23)

### [0.6.1](https://github.com/equinor/sepes-api/compare/0.6.0...0.6.1) (2021-08-20)

## [0.6.0](https://github.com/equinor/sepes-api/compare/0.5.2...0.6.0) (2021-08-18)


### ⚠ BREAKING CHANGES

* url for deleting study specific dataset is now on api/studies/datasets/studyspecific/{datasetId}

* results and learnings and study specific datasets ([#746](https://github.com/equinor/sepes-api/issues/746)) ([ccd6da9](https://github.com/equinor/sepes-api/commit/ccd6da9852b44263401b4ed6688d706717cf0f68)), closes [#742](https://github.com/equinor/sepes-api/issues/742) [#744](https://github.com/equinor/sepes-api/issues/744)

### [0.5.2](https://github.com/equinor/sepes-api/compare/0.5.1...0.5.2) (2021-08-10)

### [0.5.1](https://github.com/equinor/sepes-api/compare/0.5.0...0.5.1) (2021-08-02)


### Bug Fixes

* filter out dash in storage account name ([#736](https://github.com/equinor/sepes-api/issues/736)) ([63592a0](https://github.com/equinor/sepes-api/commit/63592a08b2b13a363dbb59b9f585d7c65bb36943))
* **function:** run as managed identity ([#700](https://github.com/equinor/sepes-api/issues/700)) ([3b50e56](https://github.com/equinor/sepes-api/commit/3b50e56161a181ae63ae3df45dc66045cb05ed7a)), closes [#669](https://github.com/equinor/sepes-api/issues/669)
* reduce max length of description ([3bc80bc](https://github.com/equinor/sepes-api/commit/3bc80bc41b5a64874291a20a4e0fff9da147d797))

## [0.5.0](https://github.com/equinor/sepes-api/compare/0.4.24...0.5.0) (2021-06-30)


### ⚠ BREAKING CHANGES

* Removed support for CRUD operations for pre-approved datasets

* test: DatasetFileService

### Bug Fixes

* **study:** increase logo sas token timeout to 61 min ([#726](https://github.com/equinor/sepes-api/issues/726)) ([00c5a6f](https://github.com/equinor/sepes-api/commit/00c5a6f19e266ddafc700143c0d339829c3a5b72))
* **study:** issue with role list and admin role ([#716](https://github.com/equinor/sepes-api/issues/716)) ([15bcc16](https://github.com/equinor/sepes-api/commit/15bcc163dddb225782ca6ea9a7a9e4d4dc6cbc1e)), closes [#710](https://github.com/equinor/sepes-api/issues/710)
* **study:** participant search ([#723](https://github.com/equinor/sepes-api/issues/723)) ([66e0267](https://github.com/equinor/sepes-api/commit/66e0267298658966203b6a179d2fa6cc46f95a6c)), closes [#712](https://github.com/equinor/sepes-api/issues/712)
* **study:** wbs cache use dapper and isolate errors ([#724](https://github.com/equinor/sepes-api/issues/724)) ([ae49bfa](https://github.com/equinor/sepes-api/commit/ae49bfadd0e5ef278a88827e7db3b10df99f63c1)), closes [#720](https://github.com/equinor/sepes-api/issues/720)
* **worker:** lifespan of health service now transient ([#722](https://github.com/equinor/sepes-api/issues/722)) ([091db37](https://github.com/equinor/sepes-api/commit/091db3773dc66d17593b2ce68c537a3a09c54f36))


* improve study read rbac coverage ([#714](https://github.com/equinor/sepes-api/issues/714)) ([b031107](https://github.com/equinor/sepes-api/commit/b031107d8b7ed3c0b9fa524f2b625e14ae19ee0f)), closes [#713](https://github.com/equinor/sepes-api/issues/713)

### [0.4.24](https://github.com/equinor/sepes-api/compare/0.4.23...0.4.24) (2021-06-25)


### Bug Fixes

* **wbsvalidation:** better cache duplication handling ([#718](https://github.com/equinor/sepes-api/issues/718)) ([f32b3ff](https://github.com/equinor/sepes-api/commit/f32b3ffec4fecdea29bd0a6306ca459255f57fce)), closes [#717](https://github.com/equinor/sepes-api/issues/717)

### [0.4.23](https://github.com/equinor/sepes-api/compare/0.4.22...0.4.23) (2021-06-24)


### Bug Fixes

* prevent fail in resource naming  ([#709](https://github.com/equinor/sepes-api/issues/709)) ([7345f92](https://github.com/equinor/sepes-api/commit/7345f9253db34eee3a8beb670191f6ec94b2cb21)), closes [#707](https://github.com/equinor/sepes-api/issues/707)
* **study:** prevent setting invalid wbs if active sandbox or dataset ([#705](https://github.com/equinor/sepes-api/issues/705)) ([f909a21](https://github.com/equinor/sepes-api/commit/f909a211f6cdc97a388337245c44370834818491)), closes [#703](https://github.com/equinor/sepes-api/issues/703) [#704](https://github.com/equinor/sepes-api/issues/704)

### [0.4.22](https://github.com/equinor/sepes-api/compare/0.4.21...0.4.22) (2021-06-17)


### Bug Fixes

* participant search from employees and affiliates ([#701](https://github.com/equinor/sepes-api/issues/701)) ([35f8a80](https://github.com/equinor/sepes-api/commit/35f8a80b82d2afc74a436a2338a140976414bd44))
* **dataset:** assign custom role for dataset resource group ([#699](https://github.com/equinor/sepes-api/issues/699)) ([9a4d9af](https://github.com/equinor/sepes-api/commit/9a4d9af4e5c1389ac3b2b6039307cbc9fddb4c8d)), closes [#692](https://github.com/equinor/sepes-api/issues/692)

### [0.4.21](https://github.com/equinor/sepes-api/compare/0.4.20...0.4.21) (2021-06-10)


### Bug Fixes

* **cypress:** use group access from token for mock user ([#691](https://github.com/equinor/sepes-api/issues/691)) ([7c84d67](https://github.com/equinor/sepes-api/commit/7c84d675e9c0c321c74edaa8c91a6d3482f05a01))
* **study:** details response also including datasets not in any sandbox ([#695](https://github.com/equinor/sepes-api/issues/695)) ([c3a972a](https://github.com/equinor/sepes-api/commit/c3a972a7756e8e5eab1f3bb0072463ac10daf6af)), closes [#694](https://github.com/equinor/sepes-api/issues/694)
* **vm:** no public IP was created for VM ([#696](https://github.com/equinor/sepes-api/issues/696)) ([11f84e8](https://github.com/equinor/sepes-api/commit/11f84e89a2f4014f147715dc7ac184f1355f9d97)), closes [#693](https://github.com/equinor/sepes-api/issues/693)

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
