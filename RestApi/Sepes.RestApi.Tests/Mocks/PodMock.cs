using System;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Tests.Mocks
{
    internal class PodMock : IPodService
    {
        public Task addNsg(string securityGroupName, string subnetName, string networkId)
        {
            throw new NotImplementedException();
        }

        public Task<Pod> CreateNewPod(string name, int userId)
        {
            return Task.FromResult(new Pod(42, name, userId));
        }

        public Task<UInt16> deleteUnused()
        {
            throw new System.NotImplementedException();
        }

        public Task removeNsg(string securityGroupName, string subnetName, string networkId)
        {
            throw new NotImplementedException();
        }

        public Task switchNsg(string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId)
        {
            throw new NotImplementedException();
        }
    }
}
