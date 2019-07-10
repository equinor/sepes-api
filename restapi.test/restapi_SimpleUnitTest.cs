using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sepes.restapi.ValuesController; //Add the controller to this test
using System.Linq; //Adds some mathematics


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
            int[] testProducts = {2,3};
            var controller = new ValuesController(); //Make the controller object
            ActionResult<int> result = controller.AddNumbers(testProducts[0], testProducts[1]); //Call funtion and pass in variables
            int resultReal = testProducts.Sum();          


            Assert.AreEqual(resultReal, result.Value);

        }
    }
}