using Azure.Storage;
using Microsoft.Azure.Management.Storage.Fluent;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;

namespace Sepes.Azure.Util
{
    public static class AzureStorageUtils
    {
        public static StorageSharedKeyCredential CreateCredentialFromConnectionString(string connectionString)
        {
            return new StorageSharedKeyCredential(
                GetKeyValueFromConnectionString(connectionString, "AccountName"),
                GetKeyValueFromConnectionString(connectionString, "AccountKey"));
        }

        public static string GetAccountName(AzureStorageAccountConnectionParameters parameters)
        {
            if (!String.IsNullOrWhiteSpace(parameters.ConnectionString))
            {
                return GetKeyValueFromConnectionString(parameters.ConnectionString, "AccountName");
            }
            else if (!String.IsNullOrWhiteSpace(parameters.StorageAccountName))
            {
                return parameters.StorageAccountName;
            }

            throw new ArgumentException("Neither connection string or account name specified");
        }

        public static string GetKeyValueFromConnectionString(string connectionString, string key)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("Connection string not set");
            }

            var settings = new Dictionary<string, string>();
            var splitted = connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new char[] { '=' }, 2);

                if (splittedNameValue.Length > 1)
                {
                    settings.Add(splittedNameValue[0], splittedNameValue[1]);
                }
                else
                {
                    throw new ArgumentException("Connection string in wrong format");
                }
                
            }

            return settings[key];
        }  
      

        public static async Task<string> GetStorageAccountKeyByName(IStorageAccount account, string keyName = "key1", CancellationToken cancellationToken = default)
        {
            var keys = await account.GetKeysAsync(cancellationToken);

            foreach (var curKey in keys)
            {
                if (curKey.KeyName == keyName)
                {
                    return curKey.Value;
                }
            }

            return null;
        }
    }
}
