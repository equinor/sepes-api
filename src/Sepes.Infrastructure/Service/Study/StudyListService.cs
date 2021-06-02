using AutoMapper;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyListService : IStudyListService
    {
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IStudyLogoReadService _studyLogoReadService;
        readonly IStudyPermissionService _studyPermissionService;
        readonly IStudyListModelService _studyRawQueryModelService;
 

        public StudyListService(IMapper mapper, IUserService userService, IStudyLogoReadService studyLogoReadService, IStudyPermissionService studyPermissionService, IStudyListModelService studyRawQueryModelService)         
        {
            _mapper = mapper;
            _userService = userService;
            _studyLogoReadService = studyLogoReadService;
            _studyPermissionService = studyPermissionService;
            _studyRawQueryModelService = studyRawQueryModelService;
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync()
        {
            var studies = await _studyRawQueryModelService.GetListAsync();

            if (studies.Any())
            {
                await _studyLogoReadService.DecorateLogoUrlsWithSAS(studies);
            }

            return studies;
        }       
    }
}
