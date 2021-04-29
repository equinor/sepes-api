using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyCreateService : StudyServiceBase, IStudyCreateService
    {
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public StudyCreateService(SepesDbContext db, IMapper mapper, ILogger<StudyCreateService> logger,
            IUserService userService,
            IStudyModelService studyModelService,
            IStudyLogoService studyLogoService,
            IDatasetCloudResourceService datasetCloudResourceService)
            : base(db, mapper, logger, userService, studyModelService, studyLogoService)
        {
            _datasetCloudResourceService = datasetCloudResourceService;
        }      

        public async Task<StudyDetailsDto> CreateAsync(StudyCreateDto newStudyDto, IFormFile logo = null, CancellationToken cancellation = default)
        {           
            StudyAccessUtil.HasAccessToOperationOrThrow(await _userService.GetCurrentUserAsync(), UserOperation.Study_Create);
            GenericNameValidation.ValidateName(newStudyDto.Name);

            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserAsync();
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);

            studyDb = await _studyModelService.AddAsync(studyDb);
                     
            await _datasetCloudResourceService.CreateResourceGroupForStudySpecificDatasetsAsync(studyDb, cancellation);

            if (logo != null)
            {
                studyDb.LogoUrl = await _studyLogoService.AddLogoAsync(studyDb.Id, logo);               
                await _db.SaveChangesAsync();
            }

            return await GetStudyDetailsAsync(studyDb.Id);
        }        

        void MakeCurrentUserOwnerOfStudy(Study study, UserDto user)
        {
            study.StudyParticipants = new List<StudyParticipant>
            {
                new StudyParticipant() { UserId = user.Id, RoleName = StudyRoles.StudyOwner, Created = DateTime.UtcNow, CreatedBy = user.UserName }
            };
        }
    }
}
