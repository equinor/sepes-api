using System;

namespace Sepes.Azure.Dto
{
    public class AzureStorageAccountConnectionParameters
    {
        public string ConnectionString { get; private set; }

        public string StorageAccountResourceGroup { get; private set; }

        public string StorageAccountId { get; private set; }
        public string StorageAccountName { get; private set; }

        public static AzureStorageAccountConnectionParameters CreateUsingConnectionString(string connectionString)
        {
            return new AzureStorageAccountConnectionParameters() { ConnectionString = connectionString };
        }

        public static AzureStorageAccountConnectionParameters CreateUsingResourceGroupAndAccountName(string resourceGroup, string accountName)
        {
            return new AzureStorageAccountConnectionParameters() { StorageAccountResourceGroup = resourceGroup, StorageAccountName = accountName };
        }

        public static AzureStorageAccountConnectionParameters CreateUsingAccountId(string accountId)
        {
            return new AzureStorageAccountConnectionParameters() { StorageAccountId = accountId };
        }

       

        public bool IsDevelopmentStorage
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ConnectionString) && ConnectionString == "UseDevelopmentStorage=true";

            }
        }
    }
}
