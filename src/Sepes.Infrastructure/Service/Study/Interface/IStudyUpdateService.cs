﻿using Sepes.Infrastructure.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyUpdateService
    { 
        Task<StudyDetailsDto> UpdateMetadataAsync(int studyId, StudyDto newStudy);
        Task<StudyResultsAndLearningsDto> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings);
    }
}
