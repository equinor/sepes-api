using Moq;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Constants;

namespace Sepes.Tests.Services.DomainServices.VirtualMachine
{
    public class VirtualMachineSizeServiceTests : VirtualMachineSizeServiceBase
    {
        public VirtualMachineSizeServiceTests()
           : base()
        {
        }
            [Fact]
            public async void GetCalculateVmPrice_ShouldReturnPrice()
            {
                await GetCalculateVmPrice_ShouldReturnPrice_RefreshAndSeedTestDatabase();
                var virtualMachineSizeService = VirtualMachineMockServiceFactory.GetVirtualMachineSizeService(_serviceProvider);
                var priceOfVm = await virtualMachineSizeService.CalculateVmPrice(1, new CalculateVmPriceUserInputDto { Size = "Size1", DataDisks=new List<string>(new string[] { "Disk1"}), OperatingSystem="windows" });

                Assert.Equal("15", priceOfVm.ToString());
            }

    }
}

