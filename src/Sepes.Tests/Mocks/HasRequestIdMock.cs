using System;
using System.Collections.Generic;
using System.Text;
using Sepes.Infrastructure.Interface;

namespace Sepes.Tests.Mocks
{
    public class HasRequestIdMock : IHasRequestId
    {
        public string RequestId() { return "requestId"; }
    }
}
