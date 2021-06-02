using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyRawQueryReadService : IStudyRawQueryReadService
    {       
        readonly IStudyLogoReadService _studyLogoReadService;
        readonly IStudyRawQueryModelService _studyRawQueryModelService;

        public StudyRawQueryReadService(IStudyLogoReadService studyLogoReadService, IStudyRawQueryModelService studyRawQueryModelService)         
        {
            _studyLogoReadService = studyLogoReadService;
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
