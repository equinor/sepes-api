using Microsoft.Extensions.DependencyInjection;
using Sepes.Azure.Service.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Test.Common.ServiceMockFactories;
using Sepes.Tests.Common.Extensions;
using Sepes.Tests.Common.ServiceMockFactories.Infrastructure;

namespace Sepes.RestApi.IntegrationTests.Setup.Scenarios
{
    public class AzureServicesMockedAndSucceeding : IMockServicesForScenarioProvider
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            //NON-AZURE, BUT EXTERNAL
            serviceCollection.SwapTransient<IWbsApiService>(provider => WbsApiMockServiceFactory.GetService(true, false));
            serviceCollection.SwapTransient<IAzureRoleAssignmentService>(provider => AzureRoleAssignmentMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureKeyVaultSecretService>(provider => AzureKeyVaultSecretMockServiceFactory.CreateBasicForResourceCreate().Object);
            serviceCollection.SwapTransient<IAzureResourceGroupService>(provider => AzureResourceGroupMockServiceFactory.CreateBasicForCreate().Object);  

            //NETWORK
            serviceCollection.SwapTransient<IAzureNetworkSecurityGroupService>(provider => AzureNetworkSecurityGroupMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureNetworkSecurityGroupRuleService>(provider => AzureNetworkSecurityGroupRuleMockServiceFactory.CreateWhereRuleSetToReturnsFalse().Object);
            serviceCollection.SwapTransient<IAzureVirtualNetworkService>(provider => AzureVirtualNetworkMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureBastionService>(provider => AzureBastionMockServiceFactory.CreateBasicForCreate().Object);

            //VIRTUAL MACHINE
            serviceCollection.SwapTransient<IAzureVirtualMachineService>(provider => AzureVirtualMachineMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureVirtualMachineExtendedInfoService>(provider => AzureVirtualMachineExtendedInfoMockServiceFactory.CreateBasicForCreate().Object);

            //STORAGE
            serviceCollection.SwapTransient<IAzureBlobStorageService>(provider => AzureBlobStorageMockServiceFactory.CreateBasicBlobStorageServiceForResourceCreation().Object);
            serviceCollection.SwapTransient<IAzureBlobStorageUriBuilderService>(provider => AzureBlobStorageMockServiceFactory.CreateBasicUriBuilderServiceForCreate().Object);
            serviceCollection.SwapTransient<IAzureStorageAccountService>(provider => AzureStorageAccountMockServiceFactory.CreateBasicForCreate().Object);         
            serviceCollection.SwapTransient<IAzureStorageAccountNetworkRuleService>(provider => AzureStorageAccountNetworkRuleMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureStorageAccountCorsRuleService>(provider => AzureStorageAccountCorsRuleMockServiceFactory.CreateBasicForCreate().Object);          

        }
    }
}
