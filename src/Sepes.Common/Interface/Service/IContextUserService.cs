namespace Sepes.Common.Interface
{
    public interface IContextUserService
    {
        public bool IsEmployee();

        public bool IsAdmin();

        public bool IsSponsor();

        public bool IsDatasetAdmin();

       
    }
}
