using System.Collections.Generic;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using System.Linq;
using System;

namespace Sepes.RestApi.Services
{

    // The main job of the Study Service is to own and keep track Study and Pod state.
    // This service is the only one that can change the internal representation of Study and pod.
    // It's also responsible of saving new changes. 
    public interface IStudyService
    {
        // Get the list of studies based on a user.
        IEnumerable<Study> GetStudies(User user, bool archived);

        // Makes changes to the meta data of a study.
        // If based is null it means its a new study.
        // This call will only succeed if based is the same as the current version of that study.
        // This method will only update the metadata about a study and will not make changes to the azure resources.
        Task<Study> Save(Study newStudy, Study based);
        Task<Study> DeletePod(Pod pod);
    }

    public class StudyService : IStudyService
    {
        ISepesDb _db;
        IPodService _podService;
        HashSet<Study> _studies;
        ushort highestPodId;


        public StudyService(ISepesDb dbService, IPodService podService)
        {
            _db = dbService;
            _podService = podService;

            _studies = new HashSet<Study>();
            highestPodId = 0;
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
                _studies.Add(study);
            }
            else if (_studies.Contains(based))
            {
                study = await UpdatePods(based, study);
                await _db.UpdateStudy(study);

                _studies.Remove(based);
                _studies.Add(study);
            }

            return study;
        }

        public async Task<Study> DeletePod(Pod pod)
        {
            Study study = null;

            foreach ( var iStudy in _studies.ToList())
            {
                if (iStudy.studyId == pod.studyId)
                {
                    study = iStudy;
                }

            }
            
            if (study.pods.Contains(pod))
            {
                await _podService.Delete(pod);
                //Remove pod from study
                var tempPods = study.pods.Remove(pod);
                var studynew = study.ReplacePods(tempPods);
                //Delete from database
                await _db.UpdateStudy(studynew);
                //Replace study in memory
                _studies.Remove(study);
                _studies.Add(studynew);
                return studynew;
                
            }
            else{
                return null; //Error data mismatch, pod changed or not found. Refresh and try again
            }

            //If results to 1 it implies pod is in memory and matches.
            //If 2 implies that pod is in memory but different state. Returns error to frontend telling user to refresh pod and reverify as it might be in active use
            //If checking fails to find pod it returns 0 and triggers error to frontend telling it to ask user to refresh
        }

        private async Task<Study> UpdatePods(Study based, Study study)
        {
            foreach (var pod in study.pods)
            {
                if (!based.pods.Contains(pod) || !based.suppliers.SequenceEqual(study.suppliers))
                {
                    // Check if pod is new
                    if (!pod.id.HasValue)
                    {
                        // Update list of pod
                        // generate new pod with id
                        Pod newPod = pod.NewPodId(++highestPodId);
                        var newPods = study.pods.Remove(pod).Add(newPod);
                        study = study.ReplacePods(newPods);

                        // Update Azure with Pod Service
                        await _podService.Set(newPod, null, study.suppliers, null);
                    }
                    else
                    {
                        Pod basePod = based.pods.ToList().Find(basePod => basePod.id == pod.id);
                        await _podService.Set(pod, basePod, study.suppliers, based.suppliers);
                    }
                }
            }

            return study;
        }

        public void LoadStudies()
        {
            _studies = _db.GetAllStudies().Result.ToHashSet();
            FindHighestPodId();
        }

        private void FindHighestPodId()
        {
            foreach (var study in _studies)
            {
                foreach (var pod in study.pods)
                {
                    if (pod.id > highestPodId)
                    {
                        highestPodId = (ushort) pod.id;
                    }
                }
            }
        }

    }
}
