using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Linq;

namespace Sepes.RestApi.Util
{
    public static class ApplicationInsightsUtil
    {
        public static string GetOperationId() {

            var operationId = default(string);

            try
            {
                var telemetry = new RequestTelemetry();

                TelemetryConfiguration
                    .Active
                    .TelemetryInitializers
                    .OfType<OperationCorrelationTelemetryInitializer>()
                    .Single()
                    .Initialize(telemetry);

                operationId = telemetry.Context.Operation.Id;

            }
            catch {
              
            }

            return operationId;
        }      
    }
}
