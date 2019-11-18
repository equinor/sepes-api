using System.Collections.Generic;
using System.Net;
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
        [Fact]
        public void ConstructorInput(){
            var pod = new PodInput(){
                podId = 42,
                podName = "Testpod",
                studyId = 24,
                tag = "Azure:Tags"
            };
            Assert.Equal(42, pod.podId);
            Assert.Equal("Testpod", pod.podName);
            Assert.Equal(24, pod.studyId);
            Assert.Equal("Azure:Tags", pod.tag);
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

        [Fact]
        public void TestEqualsMethod()
        {
            var user1 = new User("Name1", "test@test.com", "sponsor");
            var rule1 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var dataset1 = new DataSet("test", "/test", "twqt4yhqe.qe5w.ywyw5ywq.yq4e5yqe5y");

            var rules = new List<Rule>();
            rules.Add(rule1);
            var users = new List<User>();
            users.Add(user1);
            var datasets = new List<DataSet>();
            datasets.Add(dataset1);

            var pod = new Pod(11, "test", 1, false, rules, rules, users, datasets, datasets);
            var sameAsPod = new Pod(11, "test", 1, false, rules, rules, users, datasets, datasets);
            var updatedPod = new Pod(11, "test", 1, true, null, null, users, datasets, datasets);
            var differentPod = new Pod(12, "test2", 1, false, rules, rules, users, datasets, datasets);

            Assert.True(pod.Equals(sameAsPod));
            Assert.False(pod.Equals(updatedPod));
            Assert.False(pod.Equals(differentPod));
        }

        [Fact]
        public void TestEqualityForConversions()
        {
            var rule1 = new Rule(8080, IPAddress.Parse("1.1.1.1"));
            var rule2 = new Rule(400, IPAddress.Parse("1.1.1.1"));

            var dataset1 = new DataSet("test", "/test", "twqt4yhqe.qe5w.ywyw5ywq.yq4e5yqe5y");
            var dataset2 = new DataSet("test2", "/test2", "twqt4yhqe");

            var rules = new List<Rule>();
            rules.Add(rule1);
            rules.Add(rule2);

            var datasets = new List<DataSet>();
            datasets.Add(dataset1);
            datasets.Add(dataset2);

            var pod = new Pod(11, "pod1", 1, false, rules, rules, new List<User>(), null, null);
            var pod2 = new Pod(12, "pod2", 1);

            Assert.True(pod.Equals(pod.ToPodInput().ToPod()));
            Assert.False(pod.Equals(pod2.ToPodInput().ToPod()));
        }
    }
}
