using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Common.Util;
using System;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class StudyUpdateService : StudyServiceBase, IStudyUpdateService
    {
        readonly IStudyWbsValidationService _studyWbsValidationService;
        
        public StudyUpdateService(SepesDbContext db, IMapper mapper, ILogger<StudyUpdateService> logger, IUserService userService, IStudyModelService studyModelService, IStudyLogoService studyLogoService,
            IStudyWbsValidationService studyWbsValidationService)
            : base(db, mapper, logger, userService, studyModelService, studyLogoService)
        {
            _studyWbsValidationService = studyWbsValidationService;
        }

        public async Task<StudyDetailsDto> UpdateMetadataAsync(int studyId, StudyUpdateDto updatedStudy, IFormFile logo = null)
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
                
                await _studyWbsValidationService.ValidateForStudyCreateOrUpdate(studyFromDb);
            }

            if (updatedStudy.DeleteLogo)
            {
                if (!String.IsNullOrWhiteSpace(studyFromDb.LogoUrl))
                {
                    studyFromDb.LogoUrl = "";
                    await _studyLogoService.DeleteAsync(_mapper.Map<Study>(updatedStudy));
                }                
            }
            else if (logo != null)
            {
                studyFromDb.LogoUrl = await _studyLogoService.AddLogoAsync(studyFromDb.Id, logo);          
            }

            studyFromDb.Updated = DateTime.UtcNow;

            Validate(studyFromDb);

            await _db.SaveChangesAsync();

            return await GetStudyDetailsAsync(studyFromDb.Id);
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
