using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class PodTests
    {
        [Fact]
        public void Constructor()
        {
            var pod = new Pod(42, "test", 123);
            Assert.Equal(42, pod.id);
            Assert.Equal("test", pod.name);
            Assert.Equal(123, pod.studyId);
        }

        [Theory]
        [InlineData("test", "0-test-Network")]
        [InlineData("WithCaps", "0-WithCaps-Network")]
        [InlineData("gaps and spaces", "0-gaps-and-spaces-Network")]
        [InlineData("other symbols !#¤%&/()=", "0-other-symbols-!#¤%&/()=-Network")]
        public void NetworkName(string name, string networkName) {
            var pod = new Pod(0,name,0);
            Assert.Equal(networkName, pod.networkName);
        }

        [Theory]
        [InlineData(0, "10.1.0.0/24")]
        [InlineData(255, "10.1.255.0/24")]
        [InlineData(256, "10.2.0.0/24")]
        [InlineData(257, "10.2.1.0/24")]
        [InlineData(ushort.MaxValue, "10.256.255.0/24")]
        public void AddressSpace(ushort id, string addressSpace) {
            var pod = new Pod(id,"test",0);
            Assert.Equal(addressSpace, pod.addressSpace);
        }
    }
}
