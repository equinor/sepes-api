using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace Sepes.Tests.Util
{
    public class DatasetFirewallUtilsTest
    {
        [Theory]
        [InlineData(2, "1.1.1.1", "1.1.1.2")]
        [InlineData(1, "", "")]
        [InlineData(1, null, null)]
        [InlineData(1, "abc", "dc11")]
        public void SetDatasetFirewallRules_ShouldReturnTrue(int expectedValue, string clientIp, string serverIp)
        {
            var fireWallRules = new List<DatasetFirewallRule>(){ };
            fireWallRules.Add(new DatasetFirewallRule { Id = 1, Address = "1", Created = DateTime.Today.AddYears(-1) });
            var user = new UserDto() { Id = 1 };
            var dataset = new Dataset() { FirewallRules = fireWallRules };
            var result = DatasetFirewallUtils.SetDatasetFirewallRules(user, dataset, clientIp, serverIp);

            Assert.True(result);
            Assert.Equal(expectedValue, dataset.FirewallRules.Count);
        }
    }
}
