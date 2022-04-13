using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDeleteService
    {
        Task CloseStudyAsync(int studyId, bool deleteResources);
        Task DeleteStudyAsync(int studyId);
    }
}
