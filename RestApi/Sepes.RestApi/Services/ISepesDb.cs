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
        Task<int> createPod(Pod pod);
        /*JObject getPodList(Pod input);
        int createPod(Pod pod);
        int createUser(User user);
        string searchDatasetList(JObject search);
        string searchUserList(JObject search);
        string searchStudyList(JObject search);*/
    }
}
