using Azure.Storage.Sas;
using Moq;
using Sepes.Azure.Service.Interface;
using System;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureBlobStorageMockServiceFactory
    {

        public static Mock<IAzureBlobStorageService> CreateBasicBlobStorageServiceForResourceCreation()
        {          

            var mock = new Mock<IAzureBlobStorageService>();
            mock.Setup(us => us.SetConnectionParameters(It.IsAny<string>(), It.IsAny<string>()));
            mock.Setup(us => us.SetConnectionParameters(It.IsAny<string>()));

            mock.Setup(us => us.EnsureContainerExist(It.IsAny<string>(), It.IsAny<CancellationToken>()));

            return mock;
        }
       

        public static Mock<IAzureBlobStorageUriBuilderService> CreateBasicUriBuilderServiceForCreate()
        {
            var uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = "sepesintegration.blob.core.windows.net",
                Query = "falsesastoken"
            };

            var mock = new Mock<IAzureBlobStorageUriBuilderService>();
            mock.Setup(us => us.SetConnectionParameters(It.IsAny<string>(), It.IsAny<string>()));
            mock.Setup(us => us.SetConnectionParameters(It.IsAny<string>()));

            mock.Setup(us => us.CreateUriBuilder(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BlobContainerSasPermissions>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
          .ReturnsAsync(uriBuilder);

            return mock;
        }

       
    }
}
