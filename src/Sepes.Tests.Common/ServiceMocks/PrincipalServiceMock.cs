using Sepes.Infrastructure.Interface;

namespace Sepes.RestApi.IntegrationTests.Services
{
    public class PrincipalServiceMock : IPrincipalService
    {
        readonly bool _isEmployee;
        readonly bool _isAdmin;
        readonly bool _isSponsor;
        readonly bool _isDatasetAdmin;

        public PrincipalServiceMock(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            _isEmployee = isEmployee;
            _isAdmin = isAdmin;
            _isSponsor = isSponsor;
            _isDatasetAdmin = isDatasetAdmin;
        }      

        public bool IsEmployee()
        {
            return _isEmployee;
        }

        public bool IsAdmin()
        {
            return _isAdmin;
        }

        public bool IsSponsor()
        {
            return _isSponsor;
        }

        public bool IsDatasetAdmin()
        {
            return _isDatasetAdmin;
        }
    }
}
