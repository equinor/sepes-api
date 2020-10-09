using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IPermissionService
    {
        Task StudyCreate();
        Task StudyDelete();
        Task StudyRestrict();
        


        Task<bool> CanDeleteStudy();




    }
}
