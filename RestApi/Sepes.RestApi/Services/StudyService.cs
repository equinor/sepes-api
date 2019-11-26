using System.Collections.Generic;
using System.Threading.Tasks;
using Sepes.RestApi.Model;

using System.Text.Json;
using System.Linq;
using System;

namespace Sepes.RestApi.Services {

    // the main job of the Study Service is to own and keep track Study and Pod state.
    // This service is the only one that can change the internal representation of Study and pod.
    // It's also responsible of saving new changes. 
    public interface IStudyService
    {
        // Get the list of studies based on a user.
        IEnumerable<Study> GetStudies(User user, bool archived);

        // makes changes to the meta data of a study.
        // if based is null it means its a new study.
        // This call will only succeed if based is the same as the current version of that study.
        // This method will only update the metadata about a study and will not make changes to the azure resources.
        // Use the PodServise for that.
        Task<Study> Save(Study newStudy, Study based);
    }

    public class StudyService: IStudyService {

        ISepesDb _db;
        IPodService _podService;
        HashSet<Study> _studies;
        ushort numberOfPods;


        public StudyService(ISepesDb dbService, IPodService podService) {
            _db = dbService;
            _podService = podService;
            numberOfPods = 0;
        }

        public IEnumerable<Study> GetStudies(User user, bool archived)
        {
            return _studies.Where(study => study.archived == archived);
        }

        public async Task<Study> Save(Study newStudy, Study based)
        {
            Study study = newStudy;
            if (based == null)
            {
                study = await _db.NewStudy(newStudy);
            }

            _studies.Add(study);

            if (_studies.Contains(based))
            {
                Console.WriteLine("####### Contains based study");

                // Look for new pod
                if (study.pods.Any(pod => !pod.id.HasValue))
                {
                    Pod pod = study.pods.ToList().Find(pod => !pod.id.HasValue);
                    Pod newPod = pod.NewPodId(numberOfPods++);
                }
            }

            return study;
        }

        public void LoadStudies()
        {
            _studies = _db.GetAllStudies().Result.ToHashSet();

            numberOfPods = (ushort) _studies.Sum(study => study.pods.Count);
        }
    }

}
