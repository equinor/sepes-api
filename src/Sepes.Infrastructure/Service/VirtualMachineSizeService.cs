﻿using AutoMapper;
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
    public class VirtualMachineSizeService : IVirtualMachineSizeService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly ISandboxService _sandboxService;
        readonly IAzureResourceSkuService _azureResourceSkuService;

        public VirtualMachineSizeService(
            ILogger<VirtualMachineService> logger,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ISandboxService sandboxService,
            IAzureResourceSkuService azureResourceSkuService)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _sandboxService = sandboxService;
            _azureResourceSkuService = azureResourceSkuService;
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


        public async Task UpdateVmSizeCache(CancellationToken cancellationToken = default)
        {
            var currentUser = _userService.GetCurrentUser();
            var regionsFromDb = await _db.Regions.Include(r => r.VmSizeAssociations).ThenInclude(va => va.VmSize).Where(r => r.Disabled == false).ToListAsync();

            if (regionsFromDb == null || (regionsFromDb != null & regionsFromDb.Count() == 0))
            {
                throw new Exception($"Could not update Vm Size Cache, Regions found in DB");
            }

            foreach (var curRegionFromDb in regionsFromDb)
            {
                _logger.LogInformation($"Updating VM Size Cache for Region: {curRegionFromDb.Key}");

                try
                {
                    var resourceSkusFromAzure = await _azureResourceSkuService.GetSKUsForRegion(curRegionFromDb.Key, "virtualMachines", filterBasedOnResponseRestrictions: true, cancellationToken);

                    if (resourceSkusFromAzure == null || (resourceSkusFromAzure != null && resourceSkusFromAzure.Count() == 0))
                    {
                        throw new Exception($"No VM SKUs found in Azure for region {curRegionFromDb.Key}");
                    }

                    _logger.LogInformation($"Updating VM Size Cache for Region: {curRegionFromDb.Key}. Found {resourceSkusFromAzure.Count()} SKUs for region");

                    var existingSizeItemsForRegion = curRegionFromDb.VmSizeAssociations.ToDictionary(r => r.VmSize.Key, r => r.VmSize);

                    var validSkusFromAzure = new HashSet<string>();

                    VmSize curVmSizeInDb;

                    foreach (var curAzureSku in resourceSkusFromAzure)
                    {
                        if (existingSizeItemsForRegion.TryGetValue(curAzureSku.Name, out curVmSizeInDb))
                        {
                            curVmSizeInDb.Category = AzureVmUtil.GetSizeCategory(curAzureSku.Name);

                            if (ShouldBeExcluded(curVmSizeInDb))
                            {
                                var toRemoveFromDb = curRegionFromDb.VmSizeAssociations.FirstOrDefault(ra => ra.VmSizeKey == curVmSizeInDb.Key);
                                curRegionFromDb.VmSizeAssociations.Remove(toRemoveFromDb);
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
                            //Size item might exist in db for other region
                            curVmSizeInDb = await _db.VmSizes.FirstOrDefaultAsync(r => r.Key == curAzureSku.Name);

                            if (curVmSizeInDb == null)
                            {
                                curVmSizeInDb = new VmSize() { Key = curAzureSku.Name, CreatedBy = currentUser.UserName, Category = AzureVmUtil.GetSizeCategory(curAzureSku.Name) };
                            }
                            else
                            {
                                curVmSizeInDb.Category = AzureVmUtil.GetSizeCategory(curAzureSku.Name);
                            }

                            if (ShouldBeExcluded(curVmSizeInDb))
                            {
                                //Dont want to include these
                                continue;
                            }

                            PopulateVmSizeProps(curAzureSku, curVmSizeInDb);
                            curVmSizeInDb.DisplayText = AzureVmUtil.GetDisplayTextSizeForDropdown(curVmSizeInDb);

                            //Add to lookup
                            existingSizeItemsForRegion.Add(curAzureSku.Name, curVmSizeInDb);

                            //Add to DB
                            curRegionFromDb.VmSizeAssociations.Add(new RegionVmSize() { Region = curRegionFromDb, VmSize = curVmSizeInDb });
                        
                            await _db.SaveChangesAsync();
                            validSkusFromAzure.Add(curVmSizeInDb.Key);

                        }
                    }

                    //Delete those that are no longer present in Azure, or that does not pass the filter
                    foreach (var curDbSize in existingSizeItemsForRegion.Values)
                    {
                        if (!validSkusFromAzure.Contains(curDbSize.Key))
                        {
                            var toRemoveFromDb = curRegionFromDb.VmSizeAssociations.FirstOrDefault(ra => ra.VmSizeKey == curDbSize.Key);

                            if(toRemoveFromDb != null)
                            {
                                curRegionFromDb.VmSizeAssociations.Remove(toRemoveFromDb);
                            }                           
                        }
                    }

                    await _db.SaveChangesAsync();

                    _logger.LogInformation($"Done updating VM Size Cache for Region: {curRegionFromDb.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Update VM Size cache: Unable to update Size cache for Region {curRegionFromDb.Key}. Se inner exception for details", ex);
                    continue;
                }
            }

            _logger.LogInformation($"Deleting Vm Size entries not associated with any region");
            foreach (var curVmSize in await _db.VmSizes.Include(s => s.RegionAssociations).ToListAsync())
            {
                if (curVmSize.RegionAssociations == null || (curVmSize.RegionAssociations != null && curVmSize.RegionAssociations.Count == 0))
                {
                    _db.VmSizes.Remove(curVmSize);
                }
            }

            await _db.SaveChangesAsync();

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

        bool ShouldBeExcluded(VmSize curVmSizeInDb)
        {
            if (curVmSizeInDb.Category == "unknowncategory")
            {
                return true;
            }

            if (curVmSizeInDb.Category == "gpu" && curVmSizeInDb.Key != "Standard_NV8as_v4")
            {
                return true;
            }

            if (curVmSizeInDb.Key.Contains("_v4"))
            {
                return true;
            }

            return false;
        }
    }
}
