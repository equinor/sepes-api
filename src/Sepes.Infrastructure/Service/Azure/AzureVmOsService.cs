using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureVmOsService : AzureServiceBase, IAzureVmOsService
    {
        public AzureVmOsService(IConfiguration config, ILogger<AzureVmOsService> logger)
            : base(config, logger)
        {

        }

        public async Task<List<VmOsDto>> GetAvailableOperatingSystemsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var windowsVersions = await GetWindowsVersionsAsync(cancellationToken);
            //var linuxVersions = await GetLinuxVersions();

            return windowsVersions;
        }

     

        private async Task<List<VmOsDto>> GetWindowsVersionsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var region = "norwayeast";
            var imageList = await _azure.VirtualMachineImages.ListByRegionAsync(region, cancellationToken);

            var imageListFiltered = new Dictionary<string, IVirtualMachineImage>();

            foreach(var curImage in imageList)
            {
                if(RelevantOffer(curImage.Offer) && RelevantPublisher(curImage.PublisherName))
                {
                    if (!imageListFiltered.ContainsKey(curImage.Id))
                    {
                        imageListFiltered.Add(curImage.Id, curImage);
                    }
                }
            }

            return new List<VmOsDto>();

        }

        bool RelevantOffer(string offer)
        {
            if (offer.StartsWith("WindowsServer"))
            {
                return true;
            }

            return false;
        }

        bool RelevantPublisher(string publisher)
        {
            if (publisher.StartsWith("MicrosoftWindowsServer"))
            {
                return true;
            }

            return false;
        }

        //        async Task<List<VmOsDto> GetWindowsVersionsAsync(){


        //}
        //async Task<List<VmOsDto> GetLinuxVersions(){}


    }
}
