using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Sepes.Common.Constants.AzureVmOperatingSystemConstants;

namespace Sepes.Azure.Service
{
    public class AzureVirtualNetworkOperatingSystemService : AzureApiServiceBase, IAzureVirtualNetworkOperatingSystemService
    {
        public AzureVirtualNetworkOperatingSystemService(IConfiguration config, ILogger<AzureVirtualNetworkOperatingSystemService> logger, ITokenAcquisition tokenAcquisition)
            : base(config, logger, tokenAcquisition)
        {

        }
        
        public async Task<List<VmOsDto>> GetAvailableOperatingSystemsAsync(string region, CancellationToken cancellationToken = default)
        {
            var requestsToPerform = new List<ImageRequestProperties>() {
                CreateWindowsRequestProps("Windows 2019 Datacenter Core", region, "2019-Datacenter-Core"),
                CreateWindowsRequestProps("Windows 2019 Datacenter", region, "2019-Datacenter"),
                CreateWindowsRequestProps("Windows 2016 Datacenter Core", region, "2016-Datacenter-Server-Core"),
                CreateWindowsRequestProps("Windows 2016 Datacenter", region, "2016-Datacenter"),
                 CreateWindowsRequestProps("Windows 2016 Datacenter", region, "2016-Datacenter"),
                 CreateLinuxRequestProps("RedHat", region, Linux.RedHat7LVM.Publisher, Linux.RedHat7LVM.Offer, Linux.RedHat7LVM.Sku),
                 CreateLinuxRequestProps("Ubuntu", region, Linux.UbuntuServer1804LTS.Publisher, Linux.UbuntuServer1804LTS.Offer, Linux.UbuntuServer1804LTS.Sku),
                 CreateLinuxRequestProps("Debian", region, Linux.Debian10.Publisher, Linux.Debian10.Offer, Linux.Debian10.Sku),
                 CreateLinuxRequestProps("CentOS", region, Linux.CentOS75.Publisher, Linux.CentOS75.Offer, Linux.CentOS75.Sku),
            };

            var result = new List<VmOsDto>();

            foreach (var curRequest in requestsToPerform)
            {
                var curImages = await GetImages(curRequest, cancellationToken);

                if (curRequest.PickOnlyHighest)
                {
                    result.Add(CreateWindowsOsDto(GetHighestVersion(curImages), curRequest.PrettyName, curRequest.OsType));
                }
                else
                {
                    result.AddRange(curImages.Select(i => CreateWindowsOsDto(i, curRequest.PrettyName, curRequest.OsType)));
                }
            }

            return result;
        }
        ImageRequestProperties CreateWindowsRequestProps(string prettyName, string region, string skus, bool pickOnlyHighest = false)
        {
            return new ImageRequestProperties() { OsType = AzureVmConstants.WINDOWS, PrettyName = prettyName, Region = region, Publisher = "MicrosoftWindowsServer", Offer = "WindowsServer", Skus = skus, PickOnlyHighest = pickOnlyHighest };
        }

        ImageRequestProperties CreateLinuxRequestProps(string prettyName, string region, string publisher, string offer, string skus, bool pickOnlyHighest = false)
        {
            return new ImageRequestProperties() { OsType = AzureVmConstants.LINUX, PrettyName = prettyName, Region = region, Publisher = publisher, Offer = offer, Skus = skus, PickOnlyHighest = pickOnlyHighest };
        }

        async Task<List<VirtualMachineImageResource>> GetImages(ImageRequestProperties request, CancellationToken cancellationToken = default)
        {
            var images = await GetImages(request.Region, request.Publisher, request.Offer, request.Skus, cancellationToken);
            return images;
        }

        async Task<List<VirtualMachineImageResource>> GetImages(string region, string publisher, string offer, string skus, CancellationToken cancellationToken = default)
        {
            var imagesUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/providers/Microsoft.Compute/locations/{region}/publishers/{publisher}/artifacttypes/vmimage/offers/{offer}/skus/{skus}/versions?api-version=2020-06-01";
            var images = await GetResponse<List<VirtualMachineImageResource>>(imagesUrl, cancellationToken: cancellationToken);
            return images;
        }

        VirtualMachineImageResource GetHighestVersion(List<VirtualMachineImageResource> source)
        {
            return source.OrderByDescending(i => i.Name).FirstOrDefault();
        }

        VmOsDto CreateWindowsOsDto(VirtualMachineImageResource source, string prettyName, string osType)
        {
            return new VmOsDto() { Key = source.Name, DisplayValue = $"{prettyName} ({source.Name})", Category = osType };
        }
    }

    class ImageRequestProperties
    {
        public string PrettyName { get; set; }

        public string OsType { get; set; }

        public string Region { get; set; }
        public string Publisher { get; set; }
        public string Offer { get; set; }
        public string Skus { get; set; }

        public bool PickOnlyHighest { get; set; }
    }
}
