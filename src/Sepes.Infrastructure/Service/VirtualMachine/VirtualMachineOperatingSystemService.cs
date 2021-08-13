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

        public async Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            IEnumerable<VmOsDto> result = null;

            try
            {                
                var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox);

                result = await AvailableOperatingSystems(sandboxRegion, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }  


        public async Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(string region = null, CancellationToken cancellationToken = default)
        {
            var query = "SELECT v.[Id] as [Key], v.[DisplayValue], v.[Category], v.[Recommended]";
            query += " FROM [dbo].[RegionVmImage] r";
            query += " left join [dbo].[VmImages] v on r.[VmImageId] = v.[Id]";
            query += " where r.[RegionKey] = @region";
            query += " order by [Name] DESC";

            var result = await  _dapperQueryService.RunDapperQueryMultiple<VmOsDto>(query, new { region });

            return result;          
        }

        public async Task<VmImageDto> GetImage(int id)
        {
            var query = "SELECT v.[Id], v.[DisplayValue], v.[Category], v.[Recommended]";
            query += " FROM [dbo].[VmImages] v";
            query += " WHERE v.[Id] = @id";     

            var result = await _dapperQueryService.RunDapperQuerySingleAsync<VmImageDto>(query, new { id });

            return result;
        }
    }
}
