﻿using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class WbsCodeCacheModelServiceShould : WbsValidationServiceTestBase
    { 
        [Theory]
        [InlineData("someWbs", 60)]
        [InlineData("somewbs", 60)]
        [InlineData("SOMEWBS", 60)]
        public async Task ReturnTrueForSingleValidWbs(string wbsCode, int expiresInSeconds)
        {
            var service = await GetCacheService(wbsCode, expiresInSeconds);

            var codeExists = await service.Exists(wbsCode);

            Assert.True(codeExists);
        }

        [Fact]
        public async Task ReturnFalseIfCacheIsEmpty()
        {
            var service = await GetCacheService();

            var codeExists = await service.Exists("somewbs");

            Assert.False(codeExists);
        }

        [Fact]
        public async Task ReturnFalseIfNoRelevantMatches()
        {
            var service = await GetCacheService("someOtherWbs", 10);

            var codeExists = await service.Exists("somewbs");

            Assert.False(codeExists);
        }

        [Theory]
        [InlineData("someWbs", 60)]
        [InlineData("somewbs", 60)]
        [InlineData("SOMEWBS", 60)]
        public async Task KeepNonExpiredCodesOnClean(string wbsCode, int expiresInSeconds)
        {
            var service = await GetCacheService(wbsCode, expiresInSeconds);

            await service.Clean();
            var codeExists = await service.Exists(wbsCode);

            Assert.True(codeExists);
        }

        [Theory]
        [InlineData("someWbs", -1)]
        [InlineData("somewbs", -1)]
        [InlineData("SOMEWBS", -1)]
        public async Task DeleteExpiredCodesOnClean(string wbsCode, int expiresInSeconds)
        {
            var service = await GetCacheService(wbsCode, expiresInSeconds);

            await service.Clean();
            var codeExists = await service.Exists(wbsCode);

            Assert.False(codeExists);
        }
    }
}