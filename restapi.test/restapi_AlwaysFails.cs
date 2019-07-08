using NUnit.Framework;

namespace Tests
{
    public class AlwaysFails
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Fail();
        }
    }
}