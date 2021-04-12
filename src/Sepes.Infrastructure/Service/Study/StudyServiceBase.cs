﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyServiceBase : ServiceBase<Study>
    {
        protected readonly ILogger _logger;
        protected readonly IStudyModelService _studyModelService;
        protected readonly IStudyLogoService _studyLogoService;

        public StudyServiceBase(SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService, IStudyModelService studyModelService, IStudyLogoService studyLogoService)
            : base(db, mapper, userService)
        {
            _logger = logger;
            _studyLogoService = studyLogoService;
            _studyModelService = studyModelService;
        }      

        public async Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation)
        {
            var studyFromDb = await _studyModelService.GetByIdAsync(studyId, userOperation);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);
            return studyDto;
        }

        public async Task<StudyDetailsDto> GetStudyDetailsAsync(int studyId)
        {
            var spElapsed = Stopwatch.StartNew();
            var studyFromDb = await _studyModelService.GetForStudyDetailsAsync(studyId);
            var elapsedDb = spElapsed.ElapsedMilliseconds;
            spElapsed.Restart();

            var studyDetailsDto = _mapper.Map<StudyDetailsDto>(studyFromDb);
            var elapsedDto = spElapsed.ElapsedMilliseconds;
            spElapsed.Restart();

            await _studyLogoService.DecorateLogoUrlWithSAS(studyDetailsDto);
            var elapsedUrl1 = spElapsed.ElapsedMilliseconds;
            spElapsed.Restart();


            await StudyPermissionsUtil.DecorateDto(_userService, studyFromDb, studyDetailsDto.Permissions);
            var elapsedUrl2 = spElapsed.ElapsedMilliseconds;
            spElapsed.Restart();

            foreach (var curDs in studyDetailsDto.Datasets)
            {
                curDs.Sandboxes = curDs.Sandboxes.Where(sd => sd.StudyId == studyId).ToList();
            }
            var elapsedFiltered = spElapsed.ElapsedMilliseconds;
            spElapsed.Restart();

            return studyDetailsDto;
        }
    }
}
