using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class StudyService : StudyServiceBase, IStudyService
    {
        public StudyService(SepesDbContext db, IMapper mapper, ILogger<StudyService> logger, IUserService userService, IStudyLogoService studyLogoService)
            : base(db, mapper, logger, userService, studyLogoService)
        {           
           
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync(bool? excludeHidden = null)
        {
            List<Study> studiesFromDb;

            if (excludeHidden.HasValue && excludeHidden.Value)
            {
                studiesFromDb = await StudyBaseQueries.UnHiddenStudiesQueryable(_db).ToListAsync();
            }
            else
            {
                //Get unrestricted studies from db
                var unrestrictedStudiesTask = await StudyBaseQueries.UnHiddenStudiesQueryable(_db).ToListAsync();

                var user = await _userService.GetCurrentUserWithStudyParticipantsAsync();

                var restrictedStudiesAssociatedWithUser = await StudyBaseQueries.GetStudyParticipantsForUser(_db, user.Id).ToListAsync();
                var filteredRestrictedStudies = new Dictionary<int, Study>();

                foreach (var curStudyParticipant in restrictedStudiesAssociatedWithUser)
                {
                    if (StudyAccessUtil.HasAccessToOperationForStudy(user, curStudyParticipant.Study, UserOperation.Study_Read))
                    {
                        if (!filteredRestrictedStudies.ContainsKey(curStudyParticipant.StudyId))
                        {
                            filteredRestrictedStudies.Add(curStudyParticipant.StudyId, curStudyParticipant.Study);
                        }
                    }
                }                       

                var unrestrictedStudiesList = unrestrictedStudiesTask;

                foreach(var curUnrestricted in unrestrictedStudiesList)
                {
                    if (!filteredRestrictedStudies.ContainsKey(curUnrestricted.Id))
                    {
                        filteredRestrictedStudies.Add(curUnrestricted.Id, curUnrestricted);
                    }
                }

                studiesFromDb = filteredRestrictedStudies.Values.ToList();
            }

            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);

            await _studyLogoService.DecorateLogoUrlsWithSAS(studiesDtos);

            return studiesDtos;
        }     

        public async Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Read_ResultsAndLearnings, false);

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = studyFromDb.ResultsAndLearnings };
        }
    }
}
