using Microsoft.Extensions.DependencyInjection;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public interface IMockServicesForScenarioProvider
    {
        void RegisterServices(IServiceCollection serviceCollection);
    }
}
