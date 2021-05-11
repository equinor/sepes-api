using Sepes.Common.Dto.Azure;
using Sepes.Common.Util;

namespace Sepes.Tests.Setup
{
    public static class AzureResourceDtoFactory
    {
        public static AzureStorageAccountDto CreateStorageAccount()
        {
            var result = new AzureStorageAccountDto();
            result.Id = "resource-id";
            result.Key = "resource-key";
            result.Name = "resource-name";
            result.Region = RegionStringConverter.Convert("norwayeast");
            return result;
        }

        //public static AzureRoleAssignmentResponseDto CreateAzureRoleAssignmentResponseDto()
        //{
        //    var result = new AzureRoleAssignmentResponseDto();
        //    result.id = "id";
        //    result.name = "name";
        //    result.type = "type";         
        //    return result;
        //}
    }
}
