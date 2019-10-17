
using System.Threading.Tasks;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    // This service manage changes to Pods. Make sure they are validated and the correct azure actions are performed.
    // It do not own the Pod state and need to talk to StudyService to keep its up to date.
    public interface IPodService
    {
        // This is the big one. This function will do it all.
        // 1. Validate changes.
        // 2. Make the changes to azure.
        // 3. Update the study model.
        // Returns a list of issues. No issues mean that the changes was made successfully.
        Task<Pod> CreateNewPod(string name, int userID);
    }

    public class PodService : IPodService
    {
        private readonly ISepesDb _database;
        private readonly IAzureService _azure;

        public PodService(ISepesDb database, IAzureService azure)
        {
            _database = database;
            _azure = azure;
        }

        public async Task<Pod> CreateNewPod(string name, int studyID)
        {
            var pod = await _database.createPod(name, studyID);

            await _azure.CreateNetwork(pod.name, pod.addressSpace);
            return pod;
        }
    }
}