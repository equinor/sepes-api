using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDeleteService
    {  
        Task CloseStudyAsync(int studyId);
        Task DeleteStudyAsync(int studyId);   
    }
}
