using Microsoft.Identity.Web;
using Moq;
using Sepes.Common.Service.Interface;
using System.Net.Http;
using System.Threading;

namespace Sepes.Tests.Common.Mocks
{
    public static class TokenAquistionMockFactory
    {
        public static Mock<ITokenAcquisition> CreateDefault()
        {
            var mock = new Mock<ITokenAcquisition>();
            mock.Setup(t => t.GetAccessTokenForAppAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TokenAcquisitionOptions>())).ReturnsAsync("bogustoken");
            return mock;
        }

        public static Mock<IRequestAuthenticatorWithTokenAquistionService> CreateRequestAuthenticator()
        {
            var mock = new Mock<IRequestAuthenticatorWithTokenAquistionService>();
            mock.Setup(t => t.PrepareRequestForAppAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            return mock;
        }
    }
}