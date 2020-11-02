using Sepes.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Sepes.Tests.Mocks
{
    public class PrincipalServiceMock : IPrincipalService
    {
        public IPrincipal GetPrincipal()
        {
            throw new NotImplementedException();
        }
    }
}
