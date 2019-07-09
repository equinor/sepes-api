using NUnit.Framework;
using Sepes.restapi.ValuesController; //Add the controller to this test

namespace Tests
{
    public class SimpleUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddNumbers()
        {
            var num1 = 2;
            var num2 = 3;
            var controller = new ValuesController(); //Make a controller object
            var resultReal = num1 + num2;
            var resultController = 5;            



            Assert.AreEqual(resultReal, resultController);

        }
    }
}