using Moq;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class WbsValidationServiceShould : WbsValidationServiceTestBase
    {
        [Fact]
        public async Task ReturnTrueIfPresentInCache()
        {
            var service = GetWbsValidationService(foundInCache: true,
                foundInApi: false,
                out Mock<IWbsApiService> wbsApiServiceMock,
                out Mock<IWbsCodeCacheModelService> wbsCacheServiceMock);

            var wbsFound = await service.IsValid("somewbs");

            Assert.True(wbsFound);          
            Assert.Equal(1, wbsCacheServiceMock.Invocations.Count);
            Assert.Equal(0, wbsApiServiceMock.Invocations.Count);
        }

        [Fact]
        public async Task ReturnTrueIfPresentInApi()
        {
            var service = GetWbsValidationService(foundInCache: false,
                foundInApi: true,
                out Mock<IWbsApiService> wbsApiServiceMock,
                out Mock<IWbsCodeCacheModelService> wbsCacheServiceMock
                );

            var wbsFound = await service.IsValid("somewbs");

            Assert.True(wbsFound);
            Assert.Equal(2, wbsCacheServiceMock.Invocations.Count);
            Assert.Equal(1, wbsApiServiceMock.Invocations.Count);           
        }

        [Fact]
        public async Task ReturnTrueIfPresentInCacheAndApi()
        {
            var service = GetWbsValidationService(foundInCache: true,
                foundInApi: true,
                out Mock<IWbsApiService> wbsApiServiceMock,
                out Mock<IWbsCodeCacheModelService> wbsCacheServiceMock
                );

            var wbsFound = await service.IsValid("somewbs");

            Assert.True(wbsFound);
            Assert.Equal(1, wbsCacheServiceMock.Invocations.Count);
            Assert.Equal(0, wbsApiServiceMock.Invocations.Count);          
        }

        [Fact]
        public async Task ReturnFalseIfNotPresent()
        {
            var service = GetWbsValidationService(foundInCache: false,
                foundInApi: false,
                out Mock<IWbsApiService> wbsApiServiceMock,
                out Mock<IWbsCodeCacheModelService> wbsCacheServiceMock
                );

            var wbsFound = await service.IsValid("somewbs");

            Assert.False(wbsFound);
            Assert.Equal(1, wbsCacheServiceMock.Invocations.Count);
            Assert.Equal(1, wbsApiServiceMock.Invocations.Count);
           
        }
    }
}
