using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {
        
        Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null);
        Task<StudyDto> GetStudyByIdAsync(int id);

        Task<StudyDto> CreateStudyAsync(StudyDto newStudy);

        //Task<StudyDto> CreateStudyAsync(StudyDto newStudy, IFormFile studyLogo);

        Task<StudyDto> UpdateStudyDetailsAsync(int id, StudyDto newStudy);

        Task<StudyDto> UpdateStudyAsync(int id, StudyDto newStudy);

        Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int id);

        Task<IEnumerable<SandboxDto>> GetSandboxesByStudyIdAsync(int id);

        Task<StudyDto> AddSandboxAsync(int id, SandboxDto newSandbox);

        Task<StudyDto> RemoveSandboxAsync(int id, int sandboxId);

        Task<StudyDto> AddDatasetAsync(int id, int datasetId);

        Task<StudyDto> RemoveDatasetAsync(int id, int datasetId);

        Task<StudyDto> AddCustomDatasetAsync(int id, int datasetId, StudySpecificDatasetDto newDataset);

        Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo);

        Task<byte[]> GetLogoAsync(int id);

        /// <summary>
        /// Makes changes to the meta data of a study.
        /// If based is null it means its a new study.
        /// This call will only succeed if based is the same as the current version of that study.
        /// This method will only update the metadata about a study and will not make changes to the azure resources.
        /// </summary>
        /// <param name="newStudy">The updated or new study</param>
        /// <param name="based">The current study</param>
        //Task<StudyDto> Save(StudyDto newStudy, StudyDto based);
    }
}
