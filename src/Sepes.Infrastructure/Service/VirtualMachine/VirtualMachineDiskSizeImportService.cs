﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Azure.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineDiskSizeImportService : IVirtualMachineDiskSizeImportService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IUserService _userService;       
        readonly IAzureDiskPriceService _azureDiskPriceService;

        public VirtualMachineDiskSizeImportService(
            ILogger<VirtualMachineDiskSizeImportService> logger,
            SepesDbContext db,
            IUserService userService,
            IAzureDiskPriceService azureDiskPriceService)
        {
            _logger = logger;
            _db = db;        
            _userService = userService;          
            _azureDiskPriceService = azureDiskPriceService;
        }


        public async Task Import(CancellationToken cancellationToken = default)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var regionsFromDb = await _db.Regions.Include(r => r.DiskSizeAssociations).ThenInclude(va => va.DiskSize).Where(r => !r.Disabled).ToListAsync();

            if (regionsFromDb.Count() == 0)
            {
                throw new Exception($"Could not update Vm Disk Cache, No regions found in DB");
            }

            var diskEntriesFromAzure = await _azureDiskPriceService.GetDiskPrices(cancellationToken: cancellationToken);

            if (diskEntriesFromAzure == null || (diskEntriesFromAzure != null && diskEntriesFromAzure.Count() == 0))
            {
                throw new Exception("No VM Disk Size and Price found in Azure");
            }

            foreach (var curRegionFromDb in regionsFromDb)
            {
                _logger.LogInformation($"Updating VM Disk Cache for Region: {curRegionFromDb.Key}");

                try
                {
                    if (diskEntriesFromAzure.TryGetValue(curRegionFromDb.KeyInPriceApi, out AzureDiskPriceForRegion diskSizesForRegion))
                    {
                        _logger.LogInformation($"Updating VM Size Cache for Region: {curRegionFromDb.Key}. Found {diskSizesForRegion.Types.Count} Disk sizes for region");

                        var existingDbItemsForRegion = curRegionFromDb.DiskSizeAssociations.ToDictionary(r => r.DiskSize.Key, r => r.DiskSize);

                        var validDiskSizesFromAzure = new HashSet<string>();

                        DiskSize curDiskSizeInDb;

                        foreach (var curDiskSizeForRegion in diskSizesForRegion.Types)
                        {
                            if (existingDbItemsForRegion.TryGetValue(curDiskSizeForRegion.Key, out curDiskSizeInDb))
                            {
                                //Get updated price for VM Size
                                var regionAssociation = curDiskSizeInDb.RegionAssociations.Where(ra => ra.RegionKey == curRegionFromDb.Key).SingleOrDefault();
                                regionAssociation.Price = curDiskSizeForRegion.Value.price;

                                await _db.SaveChangesAsync();
                                validDiskSizesFromAzure.Add(curDiskSizeInDb.Key);
                            }
                            else
                            {
                                //Size item might exist in db for other region
                                curDiskSizeInDb = await _db.DiskSizes.FirstOrDefaultAsync(r => r.Key == curDiskSizeForRegion.Key);

                                if (curDiskSizeInDb == null)
                                {
                                    curDiskSizeInDb = new DiskSize()
                                    {
                                        Key = curDiskSizeForRegion.Key,
                                        Size = curDiskSizeForRegion.Value.size,
                                        DisplayText = AzureVmUtil.GetDiskSizeDisplayTextForDropdown(curDiskSizeForRegion.Value.size),
                                        CreatedBy = currentUser.UserName
                                    };
                                };

                                //Add to lookup
                                existingDbItemsForRegion.Add(curDiskSizeForRegion.Key, curDiskSizeInDb);

                                //Add to DB
                                curRegionFromDb.DiskSizeAssociations.Add(new RegionDiskSize() { Region = curRegionFromDb, DiskSize = curDiskSizeInDb, Price = curDiskSizeForRegion.Value.price });

                                await _db.SaveChangesAsync();
                                validDiskSizesFromAzure.Add(curDiskSizeForRegion.Key);

                            }
                        }

                        //Delete those that are no longer present in Azure, or that does not pass the filter
                        foreach (var curDbDiskSize in existingDbItemsForRegion.Values.Where(ds=> !validDiskSizesFromAzure.Contains(ds.Key)))
                        {
                            var toRemoveFromDb = curRegionFromDb.DiskSizeAssociations.FirstOrDefault(ra => ra.VmDiskKey == curDbDiskSize.Key);

                            if (toRemoveFromDb != null)
                            {
                                curRegionFromDb.DiskSizeAssociations.Remove(toRemoveFromDb);
                            }
                        }

                        await _db.SaveChangesAsync();

                        _logger.LogInformation($"Done updating VM Disk Size and Price Cache for Region: {curRegionFromDb.Name}");

                    }
                    else
                    {
                        _logger.LogError($"Update VM Size cache: Unable to update Size cache for Region {curRegionFromDb.Key}. No items for region in response");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Update VM Size cache: Unable to update Size cache for Region {curRegionFromDb.Key}. Se inner exception for details", ex);
                    continue;
                }
            }

            _logger.LogInformation($"Deleting Disk size entries not associated with any region");

            foreach (var curDiskSize in await _db.DiskSizes.Include(s => s.RegionAssociations).Where(ds=> ds.RegionAssociations == null || (ds.RegionAssociations != null && ds.RegionAssociations.Count == 0)).ToListAsync())
            {
                _db.DiskSizes.Remove(curDiskSize);
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation($"Done updating VM Disk size cache");
        }
    }
}
