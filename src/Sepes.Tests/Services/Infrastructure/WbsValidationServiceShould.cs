using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Sepes.Infrastructure.Service;
using Sepes.Tests.Common.Mocks;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class WbsValidationServiceShould : ServiceTestBase
    {
        [Fact]
        public async Task ReturnTrueForValidWbs()
        {
            var wbsCode = "someWbs";

            var config = _serviceProvider.GetService<IConfiguration>();
            var logger = _serviceProvider.GetService<ILogger<WbsValidationService>>();
            var tokenAquisition = TokenAquistionMockFactory.CreateDefault();
            
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()               
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )                
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{'code':'" + wbsCode + "'}"),
                })
                .Verifiable();


            var httpClient = new HttpClient(handlerMock.Object);
           
            var service = new WbsValidationService(config, logger, tokenAquisition.Object, httpClient);

            var result = await service.Exists("someWbs");
            
            Assert.True(result);
        }
        
    }
}
