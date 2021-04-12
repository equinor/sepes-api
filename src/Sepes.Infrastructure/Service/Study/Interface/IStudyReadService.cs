﻿using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyReadService
    {        
        Task<IEnumerable<StudyListItemResponse>> GetStudyListAsync();

        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDetailsDto> GetStudyDetailsAsync(int studyId);        

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);    
    }
}
