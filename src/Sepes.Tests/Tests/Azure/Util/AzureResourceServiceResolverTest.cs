using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sepes.Azure.Service;
using Sepes.Azure.Service.Interface;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Util
{
    public class AzureResourceServiceResolverRest
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public AzureResourceServiceResolverRest()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();

            Services.AddSingleton<IAzureResourceGroupService>(new Mock<IAzureResourceGroupService>().Object);
            Services.AddSingleton<IAzureVirtualNetworkService>(new Mock<IAzureVirtualNetworkService>().Object);
            Services.AddSingleton<IAzureBastionService>(new Mock<IAzureBastionService>().Object);

            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]
        public async void ResolvingServiceForResourceWithProvisioningStateShouldBeOkay()
        {
            //Trying resource that does not exist
            var shouldBeNull = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, "SomeResourceThatDoesNotExist");

            Assert.Null(shouldBeNull); 
            
            //Trying resource group
            var resourceGroupService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.ResourceGroup);

            Assert.NotNull(resourceGroupService);
            Assert.IsAssignableFrom<IAzureResourceGroupService>(resourceGroupService);

            //Trying VNet
            var vNetService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.VirtualNetwork);

            Assert.NotNull(vNetService);
            Assert.IsAssignableFrom<IAzureVirtualNetworkService>(vNetService);

            //Trying Bastion
            var bastionService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.Bastion);

            Assert.NotNull(bastionService);
            Assert.IsAssignableFrom<IAzureBastionService>(bastionService);
        }
    }
}
