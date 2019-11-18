using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Sepes.RestApi.Model;

using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Linq;

namespace Sepes.RestApi.Services {

    // the main job of the Study Service is to own and keep track Study and Pod state.
    // This service is the only one that can change the internal representation of Study and pod.
    // It's also responsible of saving new changes. 
    public interface IStudyService
    {
        // Get the list of studies based on a user.
        HashSet<Study> GetStudies(User user);

        // Get the list of archived studies based on a user.
        Task<ImmutableHashSet<Study>> GetArchived(User user);

        // makes changes to the meta data of a study.
        // if based is null it means its a new study.
        // This call will only succeed if based is the same as the current version of that study.
        // This method will only update the metadata about a study and will not make changes to the azure resources.
        // Use the PodServise for that.
        Task<Study> Save(Study newStudy, Study based);

        // Helper method for the pod service to save the state of a pod when its done.
        // Should not be used in other situations as it only updates the view not the truth.
        // Use Pod.Set()
        Task<Study> SetPod(Pod pod);
    }

    public class StudyService: IStudyService {

        ISepesDb _db;
        IPodService _podService;
        HashSet<Study> _studies;


        public StudyService(ISepesDb dbService, IPodService podService) {
            _db = dbService;
            _podService = podService;
            
            var inputStudies = GetInputStudies();
            _studies = ConvertStudies(inputStudies);

            Console.WriteLine("##### studies: "+_studies.Count);
        }

        public Task<ImmutableHashSet<Study>> GetArchived(User user)
        {
            throw new System.NotImplementedException();
        }

        public HashSet<Study> GetStudies(User user)
        {
            return _studies;
        }

        public async Task<Study> Save(Study newStudy, Study based)
        {
            int id = await _db.createStudy(newStudy.studyName, newStudy.userIds, newStudy.datasetIds);
            var study = new Study(newStudy.studyName, id, newStudy.pods, newStudy.sponsors, newStudy.suppliers, 
                                  newStudy.datasets, newStudy.archived, newStudy.userIds, newStudy.datasetIds);
            _studies.Add(study);

            return study;
        }

        public Task<Study> SetPod(Pod pod)
        {
            throw new System.NotImplementedException();
        }


        public HashSet<Study> ConvertStudies(IEnumerable<StudyInput> studies)
        {
            var array = new HashSet<Study>();
            
            foreach(StudyInput study in studies) {
                array.Add(study.ToStudy());
            }

            return array;
        }

        public HashSet<StudyInput> GetInputStudies()
        {
            string studiesJson = _db.getStudies(false).Result;

            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.PropertyNameCaseInsensitive = true;
            
            return JsonSerializer.Deserialize<HashSet<StudyInput>>(studiesJson, opt);
        }
    }
}