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

        //[Theory]
        //[InlineData("ClientIp is not an valid IP Address", "", "")]
        //[InlineData("ClientIp is not an valid IP Address", null, null)]
        //[InlineData("ClientIp is not an valid IP Address", "abc", "dc11")]
        //[InlineData("ServerIp is not an valid IP Address", "1.1.1.1", "dc11")]
        //[InlineData("ClientIp is not an valid IP Address", "1a.1.a1.1", "1.1.1.1")]
        //public void SetDatasetFirewallRules_ShouldReturnException(string expectedResult, string clientIp, string serverIp)
        //{
        //    var fireWallRules = new List<DatasetFirewallRule>() { };
        //    fireWallRules.Add(new DatasetFirewallRule { Id = 1, Address = "1", Created = DateTime.Today.AddYears(-1) });
        //    var user = new UserDto() { Id = 1 };
        //    var dataset = new Dataset() { FirewallRules = fireWallRules };

        //    var ex = Assert.Throws<ArgumentException>(() => DatasetFirewallUtils.SetDatasetFirewallRules(user, dataset, clientIp, serverIp));

        //    Assert.Equal(expectedResult, ex.Message);

        //}
    }
}
