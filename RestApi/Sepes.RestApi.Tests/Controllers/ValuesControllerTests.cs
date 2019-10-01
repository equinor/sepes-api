using Microsoft.AspNetCore.Mvc;
using Xunit;
using Sepes.RestApi.Controller; //Add the controller to this test
using System.Linq; //Adds some mathematics


namespace Sepes.RestApi.Tests.Controller
{

    public class ValuesControllerTests
    {
        [Fact]
        public void AddNumbers()
        {
            var controller = new ValuesController(); //Make the controller object
            ActionResult<int> result = controller.AddNumbers(2, 3); //Call funtion and pass in variables


            Assert.Equal(5, result.Value);

        }
    }
}