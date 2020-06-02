//using System.Net;
//using Sepes.RestApi.Model;
//using Xunit;

//namespace Sepes.Tests.Model
//{
//    public class RuleTests
//    {
//        [Fact]
//        public void Constructor()
//        {
//            var rule = new Rule(1, IPAddress.Parse("1.1.1.1"));

//            Assert.Equal(1, rule.port);
//            Assert.Equal(IPAddress.Parse("1.1.1.1").ToString(), rule.ip);
//            Assert.Equal("1.1.1.1", rule.ip.ToString());
//        }

//        [Fact]
//        public void TestEqualsMethod()
//        {
//            var rule1 = new Rule(1, IPAddress.Parse("1.1.1.1"));
//            var sameAsRule1 = new Rule(1, IPAddress.Parse("1.1.1.1"));
//            var differentRule = new Rule(2, IPAddress.Parse("1.1.1.2"));

//            Assert.True(rule1.Equals(sameAsRule1));
//            Assert.False(rule1.Equals(differentRule));
//        }
//    }
//}
