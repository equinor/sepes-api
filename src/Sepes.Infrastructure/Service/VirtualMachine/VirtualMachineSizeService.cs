using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineSizeService : IVirtualMachineSizeService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;        
        readonly ISandboxModelService _sandboxModelService;

        public VirtualMachineSizeService(          
            SepesDbContext db,
            IMapper mapper,
            ISandboxModelService sandboxModelService)
        {
            _db = db;
            _mapper = mapper;
            _sandboxModelService = sandboxModelService;        
        }

        public async Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default)
        {
            var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, Constants.UserOperation.Study_Crud_Sandbox);

            var sizes = await AvailableSizes(sandboxRegion, cancellationToken);

            return _mapper.Map<List<VmSizeLookupDto>>(sizes);
        }

        public async Task<List<VmSize>> AvailableSizes(string region, CancellationToken cancellationToken = default)
        {
            var relevantDbRegion = await _db.Regions.Include(r => r.VmSizeAssociations).ThenInclude(va => va.VmSize).Where(r => r.Key == region && !r.Disabled).AsNoTracking().SingleOrDefaultAsync();

            if (relevantDbRegion == null)
            {
                throw new Exception($"Region {region} not found or disabled.");
            }

            var sizes = relevantDbRegion.VmSizeAssociations.OrderBy(s => s.Price).Select(va => va.VmSize).ToList();

            return sizes;
        }

        public async Task<double> CalculateVmPrice(int sandboxId, CalculateVmPriceUserInputDto input, CancellationToken cancellationToken = default)
        {            
            var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, Constants.UserOperation.Study_Crud_Sandbox);
            var priceOfVm = await _db.RegionVmSize.Where(x => x.Region.Key == sandboxRegion && x.VmSizeKey == input.Size).AsNoTracking().SingleOrDefaultAsync();

            var priceOfDisks = 0.0;

            foreach (var disk in input.DataDisks)
            {           
                var priceOfDisk = await _db.RegionDiskSize.Where(x => x.Region.Key == sandboxRegion && x.VmDiskKey == disk).AsNoTracking().SingleOrDefaultAsync();
                priceOfDisks += priceOfDisk.Price;
            }
            
            return priceOfVm.Price + priceOfDisks;
        }      
    }
}
