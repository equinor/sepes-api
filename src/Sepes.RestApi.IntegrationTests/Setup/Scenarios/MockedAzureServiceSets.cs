using Microsoft.Extensions.DependencyInjection;
using Sepes.Azure.Service.Interface;
using Sepes.Test.Common.ServiceMockFactories;
using Sepes.Tests.Common.Extensions;

namespace Sepes.RestApi.IntegrationTests.Setup.Scenarios
{
    public class MockedAzureServiceSets : IMockServicesForScenarioProvider
    {
        public void RegisterServices(IServiceCollection serviceCollection)
        {
            
            serviceCollection.SwapTransient<IAzureKeyVaultSecretService>(provider => AzureKeyVaultSecretMockServiceFactory.CreateBasicForResourceCreate().Object);
            serviceCollection.SwapTransient<IAzureResourceGroupService>(provider => AzureResourceGroupMockServiceFactory.CreateBasicForCreate().Object);     
         
            serviceCollection.SwapTransient<IAzureNetworkSecurityGroupService>(provider => AzureNetworkSecurityGroupMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureNetworkSecurityGroupRuleService>(provider => AzureNetworkSecurityGroupRuleMockServiceFactory.CreateWhereRuleSetToReturnsFalse().Object);
            serviceCollection.SwapTransient<IAzureVirtualNetworkService>(provider => AzureVirtualNetworkMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureBastionService>(provider => AzureBastionMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureVirtualMachineService>(provider => AzureVirtualMachineMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureRoleAssignmentService>(provider => AzureRoleAssignmentMockServiceFactory.CreateBasicForCreate().Object);

            serviceCollection.SwapTransient<IAzureBlobStorageService>(provider => AzureBlobStorageMockServiceFactory.CreateBasicBlobStorageServiceForResourceCreation().Object);
            serviceCollection.SwapTransient<IAzureBlobStorageUriBuilderService>(provider => AzureBlobStorageMockServiceFactory.CreateBasicUriBuilderServiceForCreate().Object);

            serviceCollection.SwapTransient<IAzureStorageAccountService>(provider => AzureStorageAccountMockServiceFactory.CreateBasicForCreate().Object);         
            serviceCollection.SwapTransient<IAzureStorageAccountNetworkRuleService>(provider => AzureStorageAccountNetworkRuleMockServiceFactory.CreateBasicForCreate().Object);
            serviceCollection.SwapTransient<IAzureStorageAccountCorsRuleService>(provider => AzureStorageAccountCorsRuleMockServiceFactory.CreateBasicForCreate().Object);
        }
    }
}
