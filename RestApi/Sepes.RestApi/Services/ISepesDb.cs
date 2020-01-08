using Sepes.RestApi.Model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        //Task<string> getDatasetList();

        Task<int> createStudy(string studyName, int[] userIds, int[] datasetIds);
        Task<Pod> createPod(string name, int studyId);
        Task<string> getDatasetList();
        Task<Study> NewStudy(Study study);
        Task<bool> UpdateStudy(Study study);
        Task<IEnumerable<Study>> GetAllStudies();
    }
}
