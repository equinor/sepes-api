using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.ServiceNow;
using Sepes.Tests.Common.Mocks;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Tests.Infrastructure.Service.ServiceNow
{
    public class ServiceNowApiServiceTestBase : TestBase
    {
        protected IServiceNowApiService GetApiService(HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(SERVICE_NOW_JSON_RESPONSE),
                })
                .Verifiable();

            return new ServiceNowApiService(_serviceProvider.GetService<IConfiguration>(),
                _serviceProvider.GetService<ILogger<ServiceNowApiService>>(),
                TokenAquistionMockFactory.CreateRequestAuthenticator().Object,
                new HttpClient(httpMessageHandlerMock.Object));
        }

        const string SERVICE_NOW_JSON_RESPONSE = "{\"result\":{\"status\":\"success\",\"details\":{\"number\":\"ENQ6630825\"," +
            "\"assignment_group\":\"ITSOFTWARE-SDSCOREDEV\",\"caller_id\":\"SEPESINTUSER\",\"category\":\"Incident\"," +
            "\"cmdb_ci\":\"SEPES\",\"description\":\"testing from bon\",\"short_description\":\"testing from bon\"," +
            "\"state\":\"Open\",\"sys_id\":\"dd90b2eb871101145e23ea880cbb35ca\",\"urgency\":\"2-Moderate\"}}}";
    }
}
