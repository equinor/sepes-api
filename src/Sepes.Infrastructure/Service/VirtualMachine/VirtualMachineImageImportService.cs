using AutoMapper;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
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
    public class VirtualMachineImageImportService : IVirtualMachineImageImportService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IUserService _userService;
        readonly IMapper _mapper;
        readonly IAzureVirtualMachineImageService _azureVirtualMachineImageService;

        public VirtualMachineImageImportService(
            ILogger<VirtualMachineImageImportService> logger,
            SepesDbContext db,
            IUserService userService,
            IMapper mapper,
            IAzureVirtualMachineImageService azureVirtualMachineImageService)
        {
            _logger = logger;
            _db = db;
            _userService = userService;
            _mapper = mapper;
            _azureVirtualMachineImageService = azureVirtualMachineImageService;
        }      

        public async Task Import(CancellationToken cancellationToken = default)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var regionsFromDb = await _db.Regions.Include(r => r.VmImageAssociations).ThenInclude(vma => vma.VmImage).Where(r => !r.Disabled).ToListAsync();

            if (regionsFromDb.Count() == 0)
            {
                throw new Exception($"Could not update Vm Image Cache, No regions found in DB");
            }

            var vmSearchProperties = await _db.VmImageSearchProperties.ToListAsync();

            if (vmSearchProperties == null || (vmSearchProperties != null && vmSearchProperties.Count() == 0))
            {
                throw new Exception("No VM Image Search Properties found in database");
            }
            
            var entryUpdatedAt = DateTime.Now;

            foreach (var curRegionFromDb in regionsFromDb)
            {
                _logger.LogInformation($"Updating VM Image Cache for Region: {curRegionFromDb.Key}");

                var existingImagesLookup = curRegionFromDb.VmImageAssociations.ToDictionary(x => x.VmImage.ForeignSystemId, x => x);

                foreach (var curSearchProperty in vmSearchProperties)
                {
                    try
                    {
                        var resultsFromAzure = await _azureVirtualMachineImageService.GetImagesAsync(curRegionFromDb.Key, curSearchProperty.Publisher, curSearchProperty.Offer, curSearchProperty.Sku, cancellationToken);

                        var wrappedResult = WrapImageResults(curSearchProperty, resultsFromAzure);

                        if (wrappedResult != null && wrappedResult.Count > 0)
                        {
                            foreach (var curImageFromAzure in wrappedResult)
                            {
                                if (existingImagesLookup.TryGetValue(curImageFromAzure.Id, out RegionVmImage existingDbAssociation))
                                {
                                    existingDbAssociation.VmImage.Name = curImageFromAzure.Name;
                                    existingDbAssociation.VmImage.Updated = entryUpdatedAt;
                                    existingDbAssociation.VmImage.Category = curSearchProperty.Category;
                                    existingDbAssociation.VmImage.DisplayValue = CreateDisplayValue(curSearchProperty);
                                    existingDbAssociation.VmImage.DisplayValueExtended = CreateDisplayValueExtended(curSearchProperty, curImageFromAzure);
                                    ApplyCommonVmImageProperties(existingDbAssociation.VmImage, curSearchProperty, curImageFromAzure, currentUser.UserName, entryUpdatedAt);
                                }
                                else
                                {
                                    //Image item might exist in db for other region, but is not yet associated with current region
                                    var vmImage = await _db.VmImages.FirstOrDefaultAsync(i => i.ForeignSystemId == curImageFromAzure.Id);

                                    var newAssociation = new RegionVmImage() { RegionKey = curRegionFromDb.Key };

                                    if (vmImage == null)
                                    {
                                        vmImage = new VmImage()
                                        {
                                            ForeignSystemId = curImageFromAzure.Id,                                          
                                            CreatedBy = currentUser.UserName,
                                            Created = entryUpdatedAt                                     
                                        };

                                        newAssociation.VmImage = vmImage;
                                    }
                                    else
                                    {                                   
                                        newAssociation.VmImageId = vmImage.Id;
                                    }

                                    ApplyCommonVmImageProperties(vmImage, curSearchProperty, curImageFromAzure, currentUser.UserName, entryUpdatedAt);

                                    existingImagesLookup.Add(curImageFromAzure.Id, newAssociation);
                                    curRegionFromDb.VmImageAssociations.Add(newAssociation);
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Update VM Image Cache: Unable to process search property {curSearchProperty.Id} ({curSearchProperty.Publisher} {curSearchProperty.Offer} {curSearchProperty.Sku}) for Region {curRegionFromDb.Key}. Se inner exception for details", ex);
                        continue;
                    }
                }

                await _db.SaveChangesAsync();

                Dictionary<Tuple<string, int>, RegionVmImage> regionToImageAssociationsToDelete = new Dictionary<Tuple<string, int>, RegionVmImage>();

                foreach (var curImageAssociation in curRegionFromDb.VmImageAssociations)
                {
                    if (curImageAssociation.VmImage.Updated != entryUpdatedAt)
                    {
                        var keyTuple = new Tuple<string, int>(curImageAssociation.RegionKey, curImageAssociation.VmImage.Id);

                        if (!regionToImageAssociationsToDelete.ContainsKey(keyTuple))
                        {
                            regionToImageAssociationsToDelete.Add(keyTuple, curImageAssociation);
                        }
                    }
                }

                foreach (var curImageAssociationToDelete in regionToImageAssociationsToDelete)
                {
                    _db.RegionVmImage.Remove(curImageAssociationToDelete.Value);
                }

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Done updating VM Image Cache for Region: {curRegionFromDb.Name}");

            }

            _logger.LogInformation($"Deleting VM Image entries not associated with any region");

            foreach (var curVmImage in await _db.VmImages.Include(s => s.RegionAssociations).ToListAsync())
            {
                if (curVmImage.RegionAssociations == null || (curVmImage.RegionAssociations != null && curVmImage.RegionAssociations.Count == 0))
                {
                    _db.VmImages.Remove(curVmImage);
                }
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation($"Done updating VM Image cache");
        }

        List<VmImageWithAdditionalProperties> WrapImageResults(VmImageSearchProperties vmImageSearchProperties, List<VirtualMachineImageResource> source)
        {
            if (source.Count == 0)
            {
                return default(List<VmImageWithAdditionalProperties>);
            }            

            var wrappedResult = _mapper.Map<List<VmImageWithAdditionalProperties>>(source);
            wrappedResult = wrappedResult.OrderByDescending(r => r.Name).ToList();
            wrappedResult[0].IsRecommended = true;
            return wrappedResult;
        }

        void ApplyCommonVmImageProperties(VmImage imageDb, VmImageSearchProperties vmImageSearchProperties, VmImageWithAdditionalProperties imageAzure, string currentUser, DateTime updatedAt)
        {
            imageDb.Name = imageAzure.Name;
            imageDb.Updated = updatedAt;
            imageDb.UpdatedBy = currentUser;
            imageDb.Category = vmImageSearchProperties.Category;
            imageDb.DisplayValue = CreateDisplayValue(vmImageSearchProperties);
            imageDb.DisplayValueExtended = CreateDisplayValueExtended(vmImageSearchProperties, imageAzure);
            imageDb.Recommended = imageAzure.IsRecommended;
        }

        string CreateDisplayValue(VmImageSearchProperties curSearchProperty)
        {
            return $"{curSearchProperty.DisplayValue}";
        }

        string CreateDisplayValueExtended(VmImageSearchProperties curSearchProperty, VirtualMachineImageResource curImageFromAzure)
        {
            return $"{curSearchProperty.DisplayValue} ({curImageFromAzure.Name})";
        }
    }

    public class VmImageWithAdditionalProperties : VirtualMachineImageResource
    {
        public bool IsRecommended { get; set; }
    }
}
