using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyUpdateService : StudyServiceBase, IStudyUpdateService
    {
        readonly IStudyLogoCreateService _studyLogoCreateService;
        readonly IStudyLogoDeleteService _studyLogoDeleteService;
        readonly IStudyWbsValidationService _studyWbsValidationService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public StudyUpdateService(SepesDbContext db,
            IMapper mapper,
            ILogger<StudyUpdateService> logger,
            IUserService userService,
            IStudyEfModelService studyEfModelService,
            IStudyLogoReadService studyLogoReadService,
            IStudyLogoCreateService studyLogoCreateService,
            IStudyLogoDeleteService studyLogoDeleteService,
            IStudyWbsValidationService studyWbsValidationService,
             IDatasetCloudResourceService datasetCloudResourceService
          )
            : base(db, mapper, logger, userService, studyEfModelService, studyLogoReadService)
        {
            _studyLogoCreateService = studyLogoCreateService;
            _studyLogoDeleteService = studyLogoDeleteService;
            _studyWbsValidationService = studyWbsValidationService;
            _datasetCloudResourceService = datasetCloudResourceService;
        }

        public async Task<Study> UpdateMetadataAsync(int studyId, StudyUpdateDto updatedStudy, IFormFile logo = null)
        {
            if (studyId <= 0)
            {
                throw new ArgumentException("Study Id was zero or negative:" + studyId);
            }

            GenericNameValidation.ValidateName(updatedStudy.Name);

            var studyFromDb = await GetStudyForUpdateAsync(studyId, UserOperation.Study_Update_Metadata);

            if (updatedStudy.Name != studyFromDb.Name)
            {
                studyFromDb.Name = updatedStudy.Name;
            }

            if (updatedStudy.Description != studyFromDb.Description)
            {
                studyFromDb.Description = updatedStudy.Description;
            }

            if (updatedStudy.Vendor != studyFromDb.Vendor)
            {
                studyFromDb.Vendor = updatedStudy.Vendor;
            }

            if (updatedStudy.Restricted != studyFromDb.Restricted)
            {
                studyFromDb.Restricted = updatedStudy.Restricted;
            }

            if (updatedStudy.WbsCode != studyFromDb.WbsCode)
            {
                studyFromDb.WbsCode = updatedStudy.WbsCode;

                await _studyWbsValidationService.ValidateForStudyUpdate(studyFromDb, await _studyModelService.HasActiveDatasetsAsync(studyFromDb.Id)
                        || await _studyModelService.HasActiveSandboxesAsync(studyFromDb.Id));


                await _datasetCloudResourceService.UpdateTagsForStudySpecificDatasetsAsync(studyFromDb);
               

                //TODO: Update for sandboxes
            }

            if (updatedStudy.DeleteLogo)
            {
                if (!String.IsNullOrWhiteSpace(studyFromDb.LogoUrl))
                {
                    studyFromDb.LogoUrl = "";
                    await _studyLogoDeleteService.DeleteAsync(_mapper.Map<Study>(updatedStudy));
                }
            }
            else if (logo != null)
            {
                studyFromDb.LogoUrl = await _studyLogoCreateService.CreateAsync(studyFromDb.Id, logo);
            }

            studyFromDb.Updated = DateTime.UtcNow;

            Validate(studyFromDb);

            await _db.SaveChangesAsync();

            return studyFromDb;
        }

        public async Task<StudyResultsAndLearningsDto> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var studyFromDb = await GetStudyForUpdateAsync(studyId, UserOperation.Study_Update_ResultsAndLearnings);

            if (resultsAndLearnings.ResultsAndLearnings != studyFromDb.ResultsAndLearnings)
            {
                studyFromDb.ResultsAndLearnings = resultsAndLearnings.ResultsAndLearnings;
            }

            var currentUser = await _userService.GetCurrentUserAsync();
            studyFromDb.Updated = DateTime.UtcNow;
            studyFromDb.UpdatedBy = currentUser.UserName;

            await _db.SaveChangesAsync();

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = studyFromDb.ResultsAndLearnings };
        }

        public async Task<Study> GetStudyForUpdateAsync(int studyId, UserOperation userOperation)
        {
            return await _studyModelService.GetByIdAsync(studyId, userOperation);
        }
    }
}
