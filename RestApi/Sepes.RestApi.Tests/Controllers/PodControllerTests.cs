using System.Threading.Tasks;
using Sepes.RestApi.Controller;
using Sepes.RestApi.Model;
using Sepes.RestApi.Tests.Mocks;
using Xunit;

namespace Sepes.RestApi.Tests.Controller
{
    public class PodControllerTests
    {
        [Fact]
        public async Task PodCreate()
        {
            //Given
            var podService = new PodMock();
            var controller = new PodController(podService);

            //When
            var pod = await controller.createPod(new PodInput
            {
                podName = "TestPod",
                studyID = 3
            });

            //Then
            Assert.Equal("TestPod", pod.name);
            Assert.Equal(3, pod.studyId);
        }
    }

}