using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sepes.RestApi.Controller; //Add the controller to this test
using System.Linq; //Adds some mathematics


namespace Sepes.RestApi.Tests.Controller
{

    public class ValuesControllerTests
    {
        [Test]
        public void AddNumbers()
        {
            var controller = new ValuesController(); //Make the controller object
            ActionResult<int> result = controller.AddNumbers(2, 3); //Call funtion and pass in variables


            Assert.AreEqual(5, result.Value);

        }
    }
}