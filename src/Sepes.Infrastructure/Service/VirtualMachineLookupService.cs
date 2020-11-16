using AutoMapper;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineLookupService : IVirtualMachineLookupService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly ISandboxService _sandboxService;
        readonly IAzureVMService _azureVmService;
        readonly IAzureResourceSkuService _azureResourceSkuService;
        readonly IAzureCostManagementService _costService;

        public VirtualMachineLookupService(
            ILogger<VirtualMachineService> logger,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ISandboxService sandboxService,
            IAzureVMService azureVmService,
            IAzureResourceSkuService azureResourceSkuService,
            IAzureCostManagementService costService)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _sandboxService = sandboxService;
            _azureVmService = azureVmService;
            _azureResourceSkuService = azureResourceSkuService;
            _costService = costService;
        }

        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }

        public async Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput, CancellationToken cancellationToken = default)
        {
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var vmPrice = await _costService.GetVmPrice(sandbox.Region, userInput.Size, cancellationToken);

            return vmPrice;
        }


        bool ShouldBeExcluded(VmSize curVmSizeInDb)
        {
            if (curVmSizeInDb.Category == "unknowncategory")
            {
                return true;
            }

            if(curVmSizeInDb.Category == "gpu" && curVmSizeInDb.Key != "Standard_NV8as_v4")
            {
                return true;
            }

            return false;
        }
        public async Task UpdateVmSizeCache(CancellationToken cancellationToken = default)
        {
            var currentUser = _userService.GetCurrentUser();
            var regionsFromDb = await _db.Regions.Include(r => r.VmSizeAssociations).ThenInclude(va => va.VmSize).Where(r => r.Disabled == false).ToListAsync();

            if (regionsFromDb == null || (regionsFromDb != null & regionsFromDb.Count() == 0))
            {
                throw new Exception($"Could not update Vm Size Cache, Regions found in DB");
            }

            foreach (var curRegionDb in regionsFromDb)
            {
                _logger.LogInformation($"Updating VM Size Cache for Region: {curRegionDb.Name}");

                try
                {
                    var resourceSkusFromAzure = await _azureResourceSkuService.GetSKUsForRegion(curRegionDb.Key, "virtualMachines", cancellationToken);

                    if (resourceSkusFromAzure == null || (resourceSkusFromAzure != null && resourceSkusFromAzure.Count() == 0))
                    {
                        throw new Exception($"No VM SKUs found in Azure for region {curRegionDb.Key}");
                    }

                    _logger.LogInformation($"Updating VM Size Cache for Region: {curRegionDb.Name}. Found {resourceSkusFromAzure.Count()} SKUs for region");

                    var existingVmSizeInDbLookup = curRegionDb.VmSizeAssociations.ToDictionary(r => r.VmSize.Key, r => r.VmSize);

                    var validSkusFromAzure = new HashSet<string>();

                    VmSize curVmSizeInDb;

                    foreach (var curAzureSku in resourceSkusFromAzure)
                    {
                        if (existingVmSizeInDbLookup.TryGetValue(curAzureSku.Name, out curVmSizeInDb))
                        {
                            curVmSizeInDb.Category = AzureVmUtil.GetSizeCategory(curAzureSku.Name);                          

                            if (ShouldBeExcluded(curVmSizeInDb))
                            {
                                var toRemoveFromDb = curRegionDb.VmSizeAssociations.FirstOrDefault(ra => ra.VmSizeKey == curVmSizeInDb.Key);
                                curRegionDb.VmSizeAssociations.Remove(toRemoveFromDb);
                                await _db.SaveChangesAsync();
                                continue;
                            }
                            else
                            {
                                PopulateVmSizeProps(curAzureSku, curVmSizeInDb);
                                curVmSizeInDb.DisplayText = AzureVmUtil.GetDisplayTextSizeForDropdown(curVmSizeInDb);
                                await _db.SaveChangesAsync();
                                validSkusFromAzure.Add(curVmSizeInDb.Key);
                            }
                        }
                        else
                        {
                            curVmSizeInDb = new VmSize() { Key = curAzureSku.Name, CreatedBy = currentUser.UserName, Category = AzureVmUtil.GetSizeCategory(curAzureSku.Name) };                          

                            if (ShouldBeExcluded(curVmSizeInDb))
                            {                              
                                //Dont want to include these
                                continue;
                            }

                            PopulateVmSizeProps(curAzureSku, curVmSizeInDb);
                            curVmSizeInDb.DisplayText = AzureVmUtil.GetDisplayTextSizeForDropdown(curVmSizeInDb);

                            //Add to lookup
                            existingVmSizeInDbLookup.Add(curAzureSku.Name, curVmSizeInDb);

                            //Add to DB
                            curRegionDb.VmSizeAssociations.Add(new RegionVmSize() { Region = curRegionDb, VmSize = curVmSizeInDb });

                            await _db.SaveChangesAsync();
                            validSkusFromAzure.Add(curVmSizeInDb.Key);
                        }                   

                    } 
                    
                    //Delete those that are no longer present in Azure, or that does not pass the filter
                    foreach(var curDbSize in existingVmSizeInDbLookup.Values)
                    {
                        if (!validSkusFromAzure.Contains(curDbSize.Key))
                        {
                            var toRemoveFromDb = curRegionDb.VmSizeAssociations.FirstOrDefault(ra => ra.VmSizeKey == curDbSize.Key);
                            curRegionDb.VmSizeAssociations.Remove(toRemoveFromDb);
                        }
                    }

                    await _db.SaveChangesAsync();

                    foreach(var curVmSize in await _db.VmSizes.Include(s=> s.RegionAssociations).ToListAsync())
                    {
                        if(curVmSize.RegionAssociations == null || (curVmSize.RegionAssociations != null && curVmSize.RegionAssociations.Count == 0))
                        {
                            _db.VmSizes.Remove(curVmSize);
                        }
                    }

                    await _db.SaveChangesAsync();

                    _logger.LogInformation($"Done updating VM Size Cache for Region: {curRegionDb.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Update VM Size cache: Unable to update Size cache for Region {curRegionDb.Key}. Se inner exception for details", ex);
                    continue;
                }
            }

            //TODO: Delete size records with no associated region?
            _logger.LogInformation($"Done updating VM Size Cache");
        }

        void PopulateVmSizeProps(ResourceSku source, VmSize target)
        {
            foreach (var curCapability in source.Capabilities)
            {
                switch (curCapability.Name)
                {
                    case "vCPUs":
                        target.NumberOfCores = Convert.ToInt32(curCapability.Value);
                        break;
                    case "MaxDataDiskCount":
                        target.MaxDataDiskCount = Convert.ToInt32(curCapability.Value);
                        break;
                    case "MaxNetworkInterfaces":
                        target.MaxNetworkInterfaces = Convert.ToInt32(curCapability.Value);
                        break;
                    case "OSVhdSizeMB":
                        target.OsDiskSizeInMB = Convert.ToInt32(curCapability.Value);
                        break;
                    case "MemoryGB":                       
                        target.MemoryGB = Convert.ToInt32(curCapability.Value);
                        break;
                }
            }
        }

        public async Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default)
        {  
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var sizes = await AvailableSizes(sandbox.Region, cancellationToken);

           return _mapper.Map<List<VmSizeLookupDto>>(sizes);          
        }     

        public async Task<List<VmSize>> AvailableSizes(string region, CancellationToken cancellationToken = default)
        {
            var relevantDbRegion = await _db.Regions.Include(r => r.VmSizeAssociations).ThenInclude(va => va.VmSize).Where(r => r.Key == region && r.Disabled == false).SingleOrDefaultAsync();

            if (relevantDbRegion == null)
            {
                throw new Exception($"Region {region} not found or disabled.");
            }

            var sizes = relevantDbRegion.VmSizeAssociations.Select(va => va.VmSize).ToList();

            return sizes;
        }

        public async Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default)
        {
            var result = new List<VmDiskLookupDto>();

            result.Add(new VmDiskLookupDto() { Key = "64", DisplayValue = "64 GB" });
            result.Add(new VmDiskLookupDto() { Key = "128", DisplayValue = "128 GB" });
            result.Add(new VmDiskLookupDto() { Key = "256", DisplayValue = "256 GB" });
            result.Add(new VmDiskLookupDto() { Key = "512", DisplayValue = "512 GB" });
            result.Add(new VmDiskLookupDto() { Key = "1024", DisplayValue = "1024 GB" });
            result.Add(new VmDiskLookupDto() { Key = "2048", DisplayValue = "2048 GB" });
            result.Add(new VmDiskLookupDto() { Key = "4096", DisplayValue = "4096 GB" });
            result.Add(new VmDiskLookupDto() { Key = "8192", DisplayValue = "8192 GB" });

            return result;
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            List<VmOsDto> result = null;

            try
            {
                var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

                result = await AvailableOperatingSystems(sandbox.Region, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default)
        {
            //var result = await  _azureOsService.GetAvailableOperatingSystemsAsync(region, cancellationToken); 

            var result = new List<VmOsDto>();

            //Windows
            result.Add(new VmOsDto() { Key = "win2019datacenter", DisplayValue = "Windows Server 2019 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2019datacentercore", DisplayValue = "Windows Server 2019 Datacenter Core", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2016datacenter", DisplayValue = "Windows Server 2016 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2016datacentercore", DisplayValue = "Windows Server 2016 Datacenter Core", Category = "windows" });

            //Linux
            result.Add(new VmOsDto() { Key = "ubuntults", DisplayValue = "Ubuntu 1804 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "ubuntu16lts", DisplayValue = "Ubuntu 1604 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "rhel", DisplayValue = "RedHat 7 LVM", Category = "linux" });
            result.Add(new VmOsDto() { Key = "debian", DisplayValue = "Debian 10", Category = "linux" });
            result.Add(new VmOsDto() { Key = "centos", DisplayValue = "CentOS 7.5", Category = "linux" });

            return result;
        }

      
    }
}
