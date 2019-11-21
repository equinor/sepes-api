using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        //Task<string> getDatasetList();

        Task<int> createStudy(string studyName, int[] userIds, int[] datasetIds);
        Task<int> updateStudy(int studyId, bool archived);
        Task<string> getStudies(bool archived);
        Task<Pod> createPod(string name, int studyId);
        Task<string> getPods(int studyId);
        Task<string> getDatasetList();
        Task<Study> SaveStudy(Study study, bool isNewStudy);
        Task<HashSet<Study>> GetAllStudies();
    }
}
