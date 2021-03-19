using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureKeyVaultSecretService
    {
        Task<Uri> AddKeyVaultSecret(string nameOfKeyVaultUrlSetting, string secretName, string secretValue);
        Task ClearAllVmPasswords(string nameOfKeyVaultUrlSetting);
        Task<string> DeleteKeyVaultSecretValue(string nameOfKeyVaultUrlSetting, string secretName, bool purge = false);
        Task<string> GetKeyVaultSecretValue(string nameOfKeyVaultUrlSetting, string secretName);
    }
}
