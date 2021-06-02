using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Infrastructure.Service
{
    public class StudyEfReadService : StudyServiceBase, IStudyEfReadService
    { 
        public StudyEfReadService(SepesDbContext db, IMapper mapper, ILogger<StudyEfReadService> logger, IUserService userService, IStudyEfModelService studyModelService, IStudyLogoReadService studyLogoReadService)
         : base(db, mapper, logger, userService, studyModelService, studyLogoReadService)
        {
       
        } 
    }
}
