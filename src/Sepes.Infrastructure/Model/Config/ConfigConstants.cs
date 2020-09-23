namespace Sepes.Infrastructure.Model.Config
{
    public static class ConfigConstants
    {
        public const string ENV_VARIABLE_PREFIX = "SEPES_";

        public const string AZ_INSTANCE = "AzureAd:Instance";
        public const string AZ_TENANT_ID = "AzureAd:TenantId";
        public const string AZ_CLIENT_ID = "AzureAd:ClientId";
        public const string AZ_CLIENT_SECRET = "AzureAd:ClientSecret";

        public const string AZ_SWAGGER_CLIENT_ID = "Swagger:ClientId";
        public const string AZ_SWAGGER_CLIENT_SECRET = "Swagger:ClientSecret";


        public const string APPI_KEY = "Appi_Key";
        public const string KEY_VAULT = "KeyVault_Url";

        public const string DB_OWNER_CONNECTION_STRING = "SepesOwner-ConnectionString";
        public const string DB_READ_WRITE_CONNECTION_STRING = "SepesRW-ConnectionString";

        public const string SUBSCRIPTION_ID = "SubscriptionId";
        public const string DISABLE_MIGRATIONS = "DisableMigrations";

        public const string CLAIM_OID = "ClaimKeys:ObjectId";
        public const string CLAIM_USERNAME = "ClaimKeys:UserName";
        public const string CLAIM_EMAIL = "ClaimKeys:Email";
        public const string CLAIM_FULLNAME = "ClaimKeys:FullName";


    }
}
