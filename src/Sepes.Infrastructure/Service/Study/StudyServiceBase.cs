using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyServiceBase : ServiceBase<Study>
    {
        protected readonly ILogger _logger;
        protected readonly IStudyLogoService _studyLogoService;

        public StudyServiceBase(SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService, IStudyLogoService studyLogoService)
            : base(db, mapper, userService)
        {
            _logger = logger;
            _studyLogoService = studyLogoService;
        }      

        public async Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, userOperation, false);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            return studyDto;
        }

        public async Task<StudyDetailsDto> GetStudyDetailsDtoByIdAsync(int studyId, UserOperation userOperation)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, userOperation, true);

            var studyDetailsDto = _mapper.Map<StudyDetailsDto>(studyFromDb);
            await _studyLogoService.DecorateLogoUrlWithSAS(studyDetailsDto);
            await StudyPermissionsUtil.DecorateDto(_userService, studyFromDb, studyDetailsDto.Permissions);

            foreach (var curDs in studyDetailsDto.Datasets)
            {
                curDs.SandboxDatasets = curDs.SandboxDatasets.Where(sd => sd.StudyId == studyId).ToList();
            }

            return studyDetailsDto;
        }
    }
}
