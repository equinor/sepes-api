using System.Collections.Generic;
using System.Threading.Tasks;
using Sepes.Infrastructure.Model.SepesSqlModels;
using System.Linq;
using System;
using Sepes.Infrastructure.Dto;

namespace Sepes.RestApi.Services
{

    // The main job of the Study Service is to own and keep track Study and Pod state.
    // This service is the only one that can change the internal representation of Study and pod.
    // It's also responsible of saving new changes. 
    public interface IStudyService
    {
        // Get the list of studies based on a user.
        IEnumerable<StudyDto> GetStudies(UserDto user, bool archived);

        /// <summary>
        /// Makes changes to the meta data of a study.
        /// If based is null it means its a new study.
        /// This call will only succeed if based is the same as the current version of that study.
        /// This method will only update the metadata about a study and will not make changes to the azure resources.
        /// </summary>
        /// <param name="newStudy">The updated or new study</param>
        /// <param name="based">The current study</param>
        Task<StudyDto> Save(StudyDto newStudy, StudyDto based);
    }

    public class StudyService : IStudyService
    {
        ISepesDb _db;
        IPodService _podService;
        HashSet<StudyDto> _studies;
        ushort highestPodId;


        public StudyService(ISepesDb dbService, IPodService podService)
        {
            _db = dbService;
            _podService = podService;

            _studies = new HashSet<StudyDto>();
            highestPodId = 0;
        }

        public IEnumerable<StudyDto> GetStudies(UserDto user, bool archived)
        {
            var studiesFromDb = _db.GetAllStudies().Result.ToHashSet();

            return studiesFromDb.Where(study => study.archived == archived);
        }

        public async Task<StudyDto> Save(StudyDto newStudy, StudyDto based)
        {
            StudyDto study = newStudy;

            if (based == null)
            {
                study = await _db.NewStudy(newStudy);
                _studies.Add(study);
            }
            else if (_studies.Contains(based))
            {
                study = await ManagePods(based, study);
                await _db.UpdateStudy(study);

                _studies.Remove(based);
                _studies.Add(study);
            }

            return study;
        }

        private async Task<StudyDto> ManagePods(StudyDto based, StudyDto study)
        {
            study = await CreateOrUpdatePods(based, study);

            if (study.archived && based.archived)
            {
                await DeletePods(based, study);
            }
            
            return study;
        }

        private async Task<StudyDto> CreateOrUpdatePods(StudyDto based, StudyDto study)
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
                        PodDto newPod = pod.NewPodId(++highestPodId);
                        var newPods = study.pods.Remove(pod).Add(newPod);
                        study = study.ReplacePods(newPods);

                        // Update Azure with Pod Service
                        await _podService.Set(newPod, null, study.suppliers, null);
                    }
                    else
                    {
                        var basePod = based.pods.ToList().Find(p => p.id == pod.id);
                        await _podService.Set(pod, basePod, study.suppliers, based.suppliers);
                    }
                }
            }

            return study;
        }

        private async Task DeletePods(StudyDto based, StudyDto study)
        {
            List<Task> deletePodTasks = new List<Task>();
            foreach (var pod in based.pods)
            {
                if (!study.pods.Contains(pod))
                {
                    deletePodTasks.Add(_podService.Set(null, pod, null, null));
                }
            }
            await Task.WhenAll(deletePodTasks.ToArray());
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
                        highestPodId = (ushort)pod.id;
                    }
                }
            }
        }

    }
}
