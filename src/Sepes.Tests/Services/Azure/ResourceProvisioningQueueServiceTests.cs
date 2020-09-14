using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service;
using Sepes.Tests.Setup;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace Sepes.Tests.Services.Azure
{
    public class ResourceProvisioningQueueServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }      

        private readonly ProvisioningQueueParentDto mockOperation = new ProvisioningQueueParentDto
        {

            CreatedBySessionId = "session123451",
            Children = new System.Collections.Generic.List<ProvisioningQueueChildDto>() {

                new ProvisioningQueueChildDto(){
                  SandboxResourceId = 1,
                  SandboxResourceOperationId = 2,
                  Status = "Updating",
                  TryCount = 0,
                  MaxTryCount = 3,
                  OperationType = CloudResourceOperationState.NOT_STARTED
                }
            }
        };

        public ResourceProvisioningQueueServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IResourceProvisioningQueueService, ResourceProvisioningQueueService>();
            ServiceProvider = Services.BuildServiceProvider();
        }    

        IResourceProvisioningQueueService Init()
        {
            var queueService = ServiceProvider.GetService<IResourceProvisioningQueueService>();   
            return queueService;
        }

        async Task Cleanup()
        {
            var queueService = ServiceProvider.GetService<IResourceProvisioningQueueService>();          
            await queueService.DeleteQueueAsync();
        }

        [Fact]
        public async void SendMessage_ShouldSendMessage()
        {
            var queueService = Init();
        
            var msgsBefore = await queueService.RecieveMessageAsync();

            Assert.Null(msgsBefore);          

            await queueService.SendMessageAsync(mockOperation);

            var msgsAfter = await queueService.RecieveMessageAsync();
            Assert.NotNull(msgsAfter);
            Assert.NotNull(msgsAfter.OriginalMessage);
            Assert.NotNull(msgsAfter.CreatedBySessionId);

            var messageChild = msgsAfter.Children.SingleOrDefault();
            Assert.NotNull(messageChild);
            Assert.Equal(1, messageChild.SandboxResourceId);
            Assert.Equal(2, messageChild.SandboxResourceOperationId);
            Assert.Equal(0, messageChild.TryCount);
            Assert.Equal(3, messageChild.MaxTryCount);
     

            await Cleanup();
        }       
       
    }
}
