using System.Net;
using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.Tests.Model
{
    public class DataSetTests
    {
        [Fact]
        public void Constructor()
        {
            var dataset = new DataSet("test", "test", "test");

            Assert.Equal("test", dataset.displayName);
            Assert.Equal("test", dataset.opaPolicy);
            Assert.Equal("test", dataset.azureReference);
        }

        [Fact]
        public void TestEqualsMethod()
        {
            var dataset1 = new DataSet("test", "test", "test");
            var sameAsDataset1 = new DataSet("test", "test", "test");
            var differentDataset = new DataSet("test2", "test2", "test2");

            Assert.True(dataset1.Equals(sameAsDataset1));
            Assert.False(dataset1.Equals(differentDataset));
        }
    }
}
