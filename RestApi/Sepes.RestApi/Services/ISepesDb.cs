using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        //Task<string> getDatasetList();

        Task<int> createStudy(string studyName, int[] userIds, int[] datasetIds);
        Task<string> getStudies(bool archived);
        Task<Pod> createPod(string name, int studyId);
    }
}
