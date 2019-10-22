using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class DatabaseConfigTests
    {
        [Fact]
        public void Constructor()
        {
            var config = new DatabaseConfig("test string");
            Assert.Equal("test string", config.connectionString);
        }
    }
}