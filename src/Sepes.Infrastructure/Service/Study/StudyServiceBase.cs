using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyServiceBase : ServiceBase<Study>
    {
        protected readonly ILogger _logger;
        protected readonly IStudyEfModelService _studyModelService;
        protected readonly IStudyLogoReadService _studyLogoReadService;

        public StudyServiceBase(SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService, IStudyEfModelService studyModelService, IStudyLogoReadService studyLogoReadService)
            : base(db, mapper, userService)
        {
            _logger = logger;         
            _studyModelService = studyModelService;
            _studyLogoReadService = studyLogoReadService;
        }              
    }
}
