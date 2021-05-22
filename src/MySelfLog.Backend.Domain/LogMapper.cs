using System;
using System.Collections.Generic;
using System.Text.Json;
using CloudEventData;
using Evento;
using MySelfLog.Backend.Domain.Commands;

namespace MySelfLog.Backend.Domain
{
    public class LogMapper
    {
        public Uri Schema => new Uri("log/1.0", UriKind.RelativeOrAbsolute);
        public Uri Source => new Uri("myselflog", UriKind.RelativeOrAbsolute);

        private readonly List<string> _dataContentTypes = new List<string> { "application/json", "application/cloudevents+json" };

        public Command Map(CloudEventRequest request)
        {
            Ensure.NotNull(request, nameof(request));
            Ensure.NotNull(request.Data, nameof(request.Data));

            if (!_dataContentTypes.Contains(request.DataContentType))
                throw new ArgumentException($"While running Map in '{nameof(CreateDiaryMapper)}' I can't recognize the DataContentType:{request.DataContentType}");
            if (!request.DataSchema.Equals(Schema) || !request.Source.Equals(Source))
                throw new ArgumentException($"While running Map in '{nameof(CreateDiaryMapper)}' I can't recognize the data (DataSchema:{request.DataSchema};Source:{request.Source})");
            var cmd = JsonSerializer.Deserialize<Log>(request.Data.ToString());

            cmd.Metadata = new Dictionary<string, string>
            {
                {"$correlationId", cmd.CorrelationId},
                {"source", request.Source.ToString()},
                {"$applies", request.Time.ToString("O")},
                {"cloudrequest-id", request.Id},
                {"schema", request.DataSchema.ToString()},
                {"content-type", request.DataContentType}
            };
            return cmd;
        }
    }
}
