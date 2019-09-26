using Newtonsoft.Json.Linq;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    public interface ISepesDb
    {
        JObject getDatasetList();

        int createStudy(Study study);
    }
}
