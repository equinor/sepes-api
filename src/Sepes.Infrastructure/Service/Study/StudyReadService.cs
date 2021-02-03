using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyReadService : StudyServiceBase, IStudyReadService
    {
        public StudyReadService(SepesDbContext db, IMapper mapper, ILogger<StudyReadService> logger, IUserService userService, IStudyModelService studyModelService, IStudyLogoService studyLogoService)
            : base(db, mapper, logger, userService, studyModelService, studyLogoService)
        {

        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync()
        {
            var studies = await _studyModelService.GetStudyListAsync();

            if (studies.Any())
            {
                await _studyLogoService.DecorateLogoUrlsWithSAS(studies);
            }

            return studies;
        }

        public async Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Read_ResultsAndLearnings, false);

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = studyFromDb.ResultsAndLearnings };
        }
    }
}
