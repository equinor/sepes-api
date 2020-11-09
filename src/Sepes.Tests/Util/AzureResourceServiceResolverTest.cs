using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
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

            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]
        public async void ResolvingServiceForResourceWithProvisioningStateShouldBeOkay()
        {
            //Trying resource group
            var shouldBeNull = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, "SomeResourceThatDoesNotExist");

            Assert.Null(shouldBeNull);    


            //Trying resource group
            var resourceGroupService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.ResourceGroup);

            Assert.NotNull(resourceGroupService);
            Assert.IsAssignableFrom<IAzureResourceGroupService>(resourceGroupService);

            //Trying VNet
            var vNetService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.VirtualNetwork);

            Assert.NotNull(vNetService);
            Assert.IsAssignableFrom<IAzureVNetService>(vNetService);


            //Trying Bastion
            var bastionService = AzureResourceServiceResolver.GetServiceWithProvisioningState(ServiceProvider, AzureResourceType.Bastion);

            Assert.NotNull(bastionService);
            Assert.IsAssignableFrom<IAzureBastionService>(bastionService);
        }
    }
}
