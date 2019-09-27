using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        JObject getDatasetList();

        int createStudy(Study study);
        JObject getPodList(Pod input);
        int createPod(Pod pod);
        int createUser(User user);
        string searchDatasetList(JObject search);
        string searchUserList(JObject search);
        string searchStudyList(JObject search);
    }
}
