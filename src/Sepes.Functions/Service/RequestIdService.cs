using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Sepes.Infrastructure.Interface;
using System.Linq;

namespace Sepes.Functions.Service
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

            if (PotentialRequestIdIsNotEmpty(_telemetryClient.Context.Operation.Id, out string requestId))
            {
                return requestId;
            }

            if (System.Diagnostics.Activity.Current != null && PotentialRequestIdIsNotEmpty(System.Diagnostics.Activity.Current.RootId, out requestId))
            {
                return requestId;
            }

            if (PotentialRequestIdIsNotEmpty(DeprecatedFallback(), out requestId))
            {
                return requestId;
            }

            throw new System.Exception("Unable to resolve Request Id from Application Insights");             
        }

        string DeprecatedFallback()
        {           
            try
            {
                var telemetry = new RequestTelemetry();

#pragma warning disable CS0618 // Type or member is obsolete
                TelemetryConfiguration
                    .Active
#pragma warning restore CS0618 // Type or member is obsolete
                    .TelemetryInitializers
                    .OfType<OperationCorrelationTelemetryInitializer>()
                    .Single()
                    .Initialize(telemetry);

                return telemetry.Context.Operation.Id;

            }
            catch
            {

            }

            return null;
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
