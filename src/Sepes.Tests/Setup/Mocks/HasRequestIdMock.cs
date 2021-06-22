using Sepes.Common.Interface;

namespace Sepes.Tests.Mocks
{
    public class HasRequestIdMock : IRequestIdService
    {
        public string GetRequestId() { return "requestId"; }
    }
}
