using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service;
using Sepes.Tests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace Sepes.Tests.Services.Azure
{
    public class AzureQueueServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }
        private const string _testQueueName = "unit-test-sandbox-resource-operations-queue";
        private readonly SandboxResourceOperationDto mockOperation = new SandboxResourceOperationDto
        {
            SandboxResourceId = 1,
            Status = "Updating",
            TryCount = 0,
            SessionId = "session123451"
        };

        public AzureQueueServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IAzureQueueService, AzureQueueService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        string createMockMessage()
        {
            SandboxResourceOperationDto operation = new SandboxResourceOperationDto
            {
                SandboxResourceId = 1,
                Status = "Updating",
                TryCount = 0,
                SessionId = "session123451"
            };

            string message = JsonConvert.SerializeObject(operation);
            return message;
        }

        IAzureQueueService Init()
        {
            var queueService = ServiceProvider.GetService<IAzureQueueService>();
            // Have to do this to avoid test interfering with production queue.
            queueService.UseTestingQueue();
            return queueService;
        }

        async Task Cleanup()
        {
            var queueService = ServiceProvider.GetService<IAzureQueueService>();
            queueService.UseTestingQueue();
            await queueService.DeleteQueue();
        }

        [Fact]
        public async void SendMessage_ShouldSendMessage()
        {
            var queueService = Init();
            string testMessage = createMockMessage();
            var msgsBefore = await queueService.PeekMessages(32);
            int noBefore = msgsBefore.ToList().Count;

            await queueService.SendMessage(mockOperation);

            var msgsAfter = await queueService.PeekMessages(32);
            int noAfter = msgsAfter.ToList().Count;

            Assert.Equal<int>(noBefore + 1, noAfter);
            await Cleanup();
        }
        
        //[Fact]
        //public async void SandboxResourceOperationToMessageString_ShouldConvertToSameFormatAsFoundInQueue()
        //{
        //    var queueService = Init();
        //    var convertedOperation = queueService.SandboxResourceOperationToMessageString(mockOperation);
        //    await queueService.SendMessage(createMockMessage());

        //    var recievedMessageResult = await queueService.RecieveMessage();

        //    Assert.Equal(recievedMessageResult.MessageText, convertedOperation);
        //    await Cleanup();
        //}

        //[Fact]
        //public async void MessageToSandboxResourceOperation_ShouldConvertToCorrectObject()
        //{
        //    var queueService = Init();
        //    await queueService.SendMessage(createMockMessage());

        //    var recievedMessageResult = await queueService.RecieveMessages(5);
        //    var recievedMessage = recievedMessageResult.First();
        //    var deserializedMessage = queueService.MessageToSandboxResourceOperation(recievedMessage);

        //    Assert.Equal<SandboxResourceOperationDto>(mockOperation, deserializedMessage);

        //    await Cleanup();
        //}
    }
}
