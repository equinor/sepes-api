using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;

namespace Sepes.RestApi.Tests.Mocks
{
    internal class PodMock : IPodService
    {
        public Task Set(Pod newPod, Pod based, IEnumerable<User> newUsers, IEnumerable<User> basedUsers)
        {
            throw new NotImplementedException();
        }
    }
}
