namespace Sepes.Infrastructure.Interface
{
    public interface IPrincipalService
    {
        public bool IsEmployee();

        public bool IsAdmin();

        public bool IsSponsor();

        public bool IsDatasetAdmin();

       
    }
}
