using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    interface IStudyService
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
}
