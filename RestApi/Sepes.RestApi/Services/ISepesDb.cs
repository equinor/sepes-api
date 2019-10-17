using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        string getDatasetList();

        int createStudy(Study study);
        string getStudies(bool archived);
        Task<Pod> createPod(string name, int studyId);
    }
}
