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
        readonly IStudyLogoReadService _studyLogoReadService; 
        readonly IStudyListModelService _studyRawQueryModelService; 

        public StudyListService(IStudyLogoReadService studyLogoReadService, IStudyListModelService studyRawQueryModelService)         
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
