using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Sepes.Infrastructure.Service;
using Sepes.Tests.Common.Mocks;
using Sepes.Tests.Setup;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class WbsValidationServiceShould : ServiceTestBase
    {
        [Theory]
        [InlineData("somewbs")]
        [InlineData("someWbs")]
        [InlineData("SOMEWBS")]
        public async Task ReturnTrueForSingleValidWbs(string wbs)
        {
            var service = GetService(wbsCodesInResponse: wbs);

            var result = await service.Exists(wbs);

            Assert.True(result);
        }

        [Theory]
        [InlineData("someWbs", "anotherWbs")]
        [InlineData("someWbs", "anotherWbs", "aThirdWbs")]
        public async Task ReturnFalseIfMultipleMatches(params string[] wbsCodesInResponse)
        {
            var service = GetService(wbsCodesInResponse : wbsCodesInResponse);

            var result = await service.Exists(wbsCodesInResponse[0]);

            Assert.False(result);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "someWbs")]
        [InlineData(HttpStatusCode.Unauthorized, "someWbs")]
        [InlineData(HttpStatusCode.NotFound, "someWbs")]
        [InlineData(HttpStatusCode.Forbidden, "someWbs")]
        public async Task ReturnFalseIfRequestFails(HttpStatusCode httpStatusCode, params string[] wbsCodesInResponse)
        {
            var service = GetService(httpStatusCode, wbsCodesInResponse: wbsCodesInResponse);

            var result = await service.Exists(wbsCodesInResponse[0]);

            Assert.False(result);
        }

        WbsValidationService GetService(HttpStatusCode httpStatusCode = HttpStatusCode.OK, params string[] wbsCodesInResponse)
        {
            var sbResponse = new StringBuilder();
            sbResponse.Append("[");

            foreach (var curWbsCode in wbsCodesInResponse)
            {
                if (sbResponse.Length > 1)
                {
                    sbResponse.Append(",");
                }
                sbResponse.Append("{'code':'" + curWbsCode + "'}");
            }

            sbResponse.Append("]");

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(sbResponse.ToString()),
                })
                .Verifiable();


            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            var config = _serviceProvider.GetService<IConfiguration>();
            var logger = _serviceProvider.GetService<ILogger<WbsValidationService>>();
            var tokenAquisition = TokenAquistionMockFactory.CreateDefault();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            return new WbsValidationService(config, logger, tokenAquisition.Object, httpClient, userService.Object);
        }
    }
}
