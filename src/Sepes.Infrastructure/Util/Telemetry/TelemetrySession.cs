using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sepes.Infrastructure.Util.Telemetry
{
    public class TelemetrySession
    {
        readonly string _eventId;
        readonly Stopwatch _sessionStopWatch;
        readonly Dictionary<string, Stopwatch> _partialOperationStopwatches;

        public TelemetrySession(string eventId)
        {
            _eventId = eventId;
            _sessionStopWatch = Stopwatch.StartNew();
            _partialOperationStopwatches = new Dictionary<string, Stopwatch>();
        }      

        public void StartPartialOperation(string operationName)
        {
            if (_partialOperationStopwatches.ContainsKey(operationName))
            {
                throw new ArgumentException($"Operation with name {operationName} allready exists");
            }

            _partialOperationStopwatches.Add(operationName, Stopwatch.StartNew());
        }

        public void StopPartialOperation(string operationName)
        {
            Stopwatch existingOperationWatch;

            if (_partialOperationStopwatches.TryGetValue(operationName, out existingOperationWatch))
            {
                existingOperationWatch.Stop();
            }
            else
            {
                throw new ArgumentException($"Operation with name {operationName} does not exists");
            }
        }

        public void StopSessionAndLog(TelemetryClient telemetryClient)
        {
            _sessionStopWatch.Stop();
            var metrics = _partialOperationStopwatches.ToDictionary(m => m.Key, m => (double)m.Value.ElapsedMilliseconds);
            metrics.Add("total_elapsed", (double)_sessionStopWatch.ElapsedMilliseconds);

            if(telemetryClient != null)
            {
                telemetryClient.TrackEvent(_eventId, metrics: metrics);
            }          
        }
    }
}
