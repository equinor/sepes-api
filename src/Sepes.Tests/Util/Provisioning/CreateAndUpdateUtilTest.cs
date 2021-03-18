using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util.Provisioning
{
    public class CreateAndUpdateUtilTest
    {
        [Fact]
        public void WillBeHandledAsCreateOrUpdate_ShouldReturnFalse()
        {
            var result = CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(new CloudResourceOperationDto {  });

            Assert.False(result);
        }

        [Fact]
        public void WillBeHandledAsCreateOrUpdate_ShouldReturnException()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(null));
            Assert.Equal("Cloud-Resource-Operation was null", ex.Message);
        }

        [Fact]
        public void WillBeHandledAsCreateOrUpdate_ShouldReturnTrue2()
        {
            var result = CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(new CloudResourceOperationDto { OperationType = CloudResourceOperationType.CREATE });

            Assert.True(result);
        }

        [Fact]
        public void WillBeHandledAsCreateOrUpdate_ShouldReturnTrue3()
        {
            var result = CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(new CloudResourceOperationDto { OperationType = CloudResourceOperationType.UPDATE });

            Assert.True(result);
        }
    }
}
