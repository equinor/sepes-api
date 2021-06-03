using Microsoft.Extensions.DependencyInjection;
using Sepes.Tests.Setup;

namespace Sepes.Tests.Services
{
    public class ServiceTestBase
    {
        protected readonly ServiceCollection _services;
        protected readonly ServiceProvider _serviceProvider;
                    
        public ServiceTestBase()
        {
            _services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            _serviceProvider = _services.BuildServiceProvider();
        }
    }
}