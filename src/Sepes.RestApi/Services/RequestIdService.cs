using Sepes.Infrastructure.Interface;
using Sepes.RestApi.Util;

namespace Sepes.RestApi.Services
{
    public class RequestIdService : IHasRequestId
    {
        public string RequestId()
        {
            return ApplicationInsightsUtil.GetOperationId();
        }
    }
}
