using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetService
    {
        
        Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync();
        Task<DatasetDto> GetDatasetByIdAsync(int id);

        //Task<StudyDto> CreateStudyAsync(StudyDto newStudy);

        //Task<StudyDto> UpdateStudyAsync(int id, StudyDto newStudy);   


        // ------------------Does actions against study--------------
        Task<StudyDto> AddDatasetToStudyAsync(int id, int datasetId);
        Task<StudyDto> RemoveDatasetFromStudyAsync(int id, int datasetId);
        Task<StudyDto> AddStudySpecificDatasetAsync(int id, StudySpecificDatasetDto newDataset);
    }
}
