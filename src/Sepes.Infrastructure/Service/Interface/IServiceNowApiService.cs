using Sepes.Common.Dto.ServiceNow;
using Sepes.Infrastructure.Service.ServiceNow;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IServiceNowApiService
    {
        Task<ServiceNowResponse> CreateEnquiry(ServiceNowEnquiryCreateDto enquiry, CancellationToken cancellationToken = default);
    }
}
