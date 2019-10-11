using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Sepes.RestApi.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Management.Fluent;

namespace Sepes.RestApi.Services
{
    public interface IAzPod
    {
        string CreatePod(int podID, string podName, string podTag, IAzure azure);
    }
}
