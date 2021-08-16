using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineOperatingSystemService : IVirtualMachineOperatingSystemService
    {
        readonly ILogger _logger;    
        readonly ISandboxModelService _sandboxModelService;
        readonly IDapperQueryService _dapperQueryService;

        public VirtualMachineOperatingSystemService(
            ILogger<VirtualMachineOperatingSystemService> logger,
            ISandboxModelService sandboxModelService,
            IDapperQueryService dapperQueryService
            )
        {     
            _logger = logger;     
            _sandboxModelService = sandboxModelService;
            _dapperQueryService = dapperQueryService;
        }

        public async Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(int sandboxId)
        {
            IEnumerable<VmOsDto> result = null;

            try
            {                
                var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox);

                result = await AvailableOperatingSystems(sandboxRegion);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }  


        async Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(string region)
        {
            var query = "SELECT v.[Id] as [Key], v.[DisplayValueExtended] as DisplayValue, v.[Category], v.[Recommended]";
            query += " FROM [dbo].[RegionVmImage] r";
            query += " left join [dbo].[VmImages] v on r.[VmImageId] = v.[Id]";
            query += " where r.[RegionKey] = @region";
            query += " order by [Recommended] DESC, [DisplayValue] DESC";

            var result = await  _dapperQueryService.RunDapperQueryMultiple<VmOsDto>(query, new { region });

            return result;          
        }

        public async Task<VmImageDto> GetImage(int id)
        {
            var query = "SELECT v.[Id], v.[DisplayValue], v.[DisplayValueExtended], v.[ForeignSystemId], v.[Category], v.[Recommended]";
            query += " FROM [dbo].[VmImages] v";
            query += " WHERE v.[Id] = @id";     

            var result = await _dapperQueryService.RunDapperQuerySingleAsync<VmImageDto>(query, new { id });

            return result;
        }
    }
}
