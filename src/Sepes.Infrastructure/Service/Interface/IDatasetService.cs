using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetService
    {
        
        Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync();
        Task<DatasetDto> GetDatasetByDatasetIdAsync(int id);
        Task<DatasetDto> GetSpecificDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId);

        //Task<StudyDto> CreateStudyAsync(StudyDto newStudy);

        //Task<StudyDto> UpdateStudyAsync(int id, StudyDto newStudy);   


        // ------------------Does actions against study--------------
        Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId);
        Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId);
        Task<StudyDto> AddStudySpecificDatasetAsync(int studyId, StudySpecificDatasetDto newDataset);
    }
}
