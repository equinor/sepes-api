//using Microsoft.Azure.WebJobs;
//using Sepes.Azure.Service.Interface;
//using System.Threading.Tasks;

//namespace Sepes.Functions
//{
//    public class WipeVmPasswordsFunction
//    {       
//        readonly IAzureKeyVaultSecretService _azureKeyVaultSecretService;

//        public WipeVmPasswordsFunction(IAzureKeyVaultSecretService azureKeyVaultSecretService)
//        {
//            _azureKeyVaultSecretService = azureKeyVaultSecretService;
//        }

//        [FunctionName("WipeVmPasswordsFunction")]
//        public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer)
//        {
//           await _azureKeyVaultSecretService.ClearAllVmPasswords("AzureVmTempPasswordStorageKeyVault_Url");                 
//        }
//    }
//}
//README: KEEPING THIS AS PART OF THE CODEBASE UNTIL WE GET A AUTOMATIC CLEANING PROCESS
