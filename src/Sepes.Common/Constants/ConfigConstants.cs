﻿namespace Sepes.Common.Constants
{
    public static class ConfigConstants
    {
        public const string ENV_VARIABLE_PREFIX = "SEPES_";
        public const string ALLOW_CORS_DOMAINS = "AllowCorsDomains";
        public const string IS_INTEGRATION_TEST = "IsIntegrationTest";

        public const string AZ_DOMAIN = "AzureAd:Domain";
        public const string AZ_INSTANCE = "AzureAd:Instance";
        public const string AZ_TENANT_ID = "AzureAd:TenantId";
        public const string AZ_CLIENT_ID = "AzureAd:ClientId";
        public const string AZ_CLIENT_SECRET = "AzureAd:ClientSecret";

        public const string RADIX_SECRET_AZ_CLIENT_ID = "AzureAd__ClientId";
        public const string RADIX_SECRET_AZ_CLIENT_SECRET = "AzureAd__ClientSecret";

        public const string AZ_SWAGGER_CLIENT_ID = "Swagger:ClientId";
        public const string AZ_SWAGGER_CLIENT_SECRET = "Swagger:ClientSecret";
        public const string APPI_KEY = "ApplicationInsightsInstrumentationKey";
        public const string KEY_VAULT = "KeyVault_Url";

        public const string AZURE_VM_TEMP_PASSWORD_KEY_VAULT = "AzureVmTempPasswordStorageKeyVault_Url";

        public const string DB_OWNER_CONNECTION_STRING = "SepesOwnerConnectionString";
        public const string DB_READ_WRITE_CONNECTION_STRING = "SepesRWConnectionString";
        public const string DB_INTEGRATION_TEST_CONNECTION_STRING = "SepesIntegrationTestConnectionString";

        public const string SUBSCRIPTION_ID = "SubscriptionId";
        public const string DISABLE_MIGRATIONS = "DisableMigrations";

        public const string RESOURCE_PROVISIONING_QUEUE_CONSTRING = "ResourceProvisioningQueueConnectionString";

        public const string STUDY_LOGO_STORAGE_CONSTRING = "StudyLogoStorageConnectionString";

        public const string MANAGED_BY = "ManagedBy";

        public const string COST_ALLOCATION_TYPE_TAG_NAME = "CostAllocationTypeTagName";
        public const string COST_ALLOCATION_CODE_TAG_NAME = "CostAllocationCodeTagName";

        public const string DISABLE_MONITORING = "DisableMonitoring";
        public const string DISABLE_CACHE_UPDATE = "DisableCacheUpdate";

        public const string EMPLOYEE_ROLE = "EmployeeRole";
        public const string EMPLOYEE_GROUP_ID = "CompanyEmployeeGroupId";
        public const string AFFILIATE_GROUP_ID = "AffiliateGroupId";

        public const string SERVER_PUBLIC_IP_URLS = "GetServerPublicIpServiceUrls";

        public const string SENSITIVE_DATA_LOGGING = "EnableSensitiveDataLogging";

        public const string CYPRESS_MOCK_USER = "CypressMockUser";

        //WBS SEARCH
        public const string WBS_DISABLE_ALL_VALIDATION = "WbsSearchDisabled";
        public const string WBS_SEARCH_API_URL = "WbsSearchBaseUrl";
        public const string WBS_SEARCH_API_SCOPE = "WbsSearchScope";
        public const string WBS_SEARCH_APIM_SUBSCRIPTION = "WbsSearchApimSubscriptionKey";

        //ServiceNow
        public const string SERVICE_NOW_API_URL = "ServiceNowApiUrl";
        public const string SERVICE_NOW_API_SCOPE = "ServiceNowApiScope";
        public const string SERVICE_NOW_APIM_SUBSCRIPTION = "ServiceNowApimSubscriptionKey";
        public const string SERVICE_NOW_CMDB_CI = "ServiceNowCmdbCi";

        public const string DATASET_STORAGEACCOUNT_ROLE_ASSIGNMENT_ID = "DatasetStorageAccountRoleAssignmentId";


    }
}
