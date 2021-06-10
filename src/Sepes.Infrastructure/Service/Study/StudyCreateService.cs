using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyCreateService : StudyServiceBase, IStudyCreateService
    {
        readonly IStudyLogoCreateService _studyLogoCreateService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;
        readonly IStudyWbsValidationService _studyWbsValidationService;

        public StudyCreateService(SepesDbContext db, IMapper mapper, ILogger<StudyCreateService> logger,
            IUserService userService,
            IStudyEfModelService studyModelService,
            IStudyLogoCreateService studyLogoCreateService,
            IStudyLogoReadService studyLogoReadService,
            IDatasetCloudResourceService datasetCloudResourceService,
            IStudyWbsValidationService studyWbsValidationService
           )
            : base(db, mapper, logger, userService, studyModelService, studyLogoReadService)
        {
            _studyLogoCreateService = studyLogoCreateService;
            _datasetCloudResourceService = datasetCloudResourceService;
            _studyWbsValidationService = studyWbsValidationService;
        }      

        public async Task<Study> CreateAsync(StudyCreateDto newStudyDto, IFormFile logo = null, CancellationToken cancellation = default)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            OperationAccessUtil.HasAccessToOperationOrThrow(currentUser, UserOperation.Study_Create);
            GenericNameValidation.ValidateName(newStudyDto.Name);

            var studyDb = _mapper.Map<Study>(newStudyDto);

       
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);
            
            await _studyWbsValidationService.ValidateForStudyCreateOrUpdate(studyDb);

            studyDb = await _studyModelService.AddAsync(studyDb);

            await _datasetCloudResourceService.CreateResourceGroupForStudySpecificDatasetsAsync(studyDb, cancellation);

            if (logo != null)
            {
                studyDb.LogoUrl = await _studyLogoCreateService.CreateAsync(studyDb.Id, logo);               
                await _db.SaveChangesAsync();
            }

            return studyDb;
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
