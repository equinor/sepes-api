using Microsoft.ApplicationInsights;
using Sepes.Infrastructure.Interface;

namespace Sepes.RestApi.Services
{
    public class RequestIdService : IRequestIdService
    {
        readonly TelemetryClient _telemetryClient;

        public RequestIdService(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public string GetRequestId()
        {
            string requestId = null;

            if (PotentialRequestIdIsNotEmpty(_telemetryClient.Context.Operation.Id, out requestId))
            {
                return requestId;
            }

            if (PotentialRequestIdIsNotEmpty(System.Diagnostics.Activity.Current.RootId, out requestId))
            {
                return requestId;
            }

            return "na";                       
        }

        bool PotentialRequestIdIsNotEmpty(string potentialRequestId, out string validRequestId)
        {
            if (!string.IsNullOrWhiteSpace(potentialRequestId))
            {
                validRequestId = potentialRequestId;
                return true;
            }

            validRequestId = null;
            return false;
        }
    }
}
