using Sepes.Common.Dto.ServiceNow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Tests.Infrastructure.Service.ServiceNow
{
    public class ServiceNowApiServiceShould : ServiceNowApiServiceTestBase
    {
        [Fact]
        public async Task ReturnResponseIfEnquirySuccessfullyCreated()
        {
            var service = GetApiService();
            var enquiry = new ServiceNowEnquiryCreateDto { Category = "incident", ShortDescription = "test" };
            var response = await service.CreateEnquiry(enquiry);
            Assert.True(response.Result.Status == "success");
        }
    }
}
