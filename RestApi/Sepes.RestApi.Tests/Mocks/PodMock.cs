using System.Threading.Tasks;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Tests.Mocks
{
    internal class PodMock : IPodService
    {
        public Task<Pod> CreateNewPod(string name, int userId)
        {
            return Task.FromResult(new Pod(42, name, userId));
        }
    }
}
