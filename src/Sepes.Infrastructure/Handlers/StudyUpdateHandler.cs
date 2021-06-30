using Microsoft.AspNetCore.Http;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Common.Util;
using Sepes.Infrastructure.Handlers.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Handlers
{
    public class StudyUpdateHandler : IStudyUpdateHandler
    {
        readonly SepesDbContext _db;
        readonly IStudyLogoCreateService _studyLogoCreateService;
        readonly IStudyLogoDeleteService _studyLogoDeleteService;
        readonly IStudyWbsValidationService _studyWbsValidationService;
        readonly IStudyWbsUpdateHandler _studyWbsUpdateHandler;

        readonly IStudyEfModelOperationsService _studyEfModelOperationsService;

        public StudyUpdateHandler(SepesDbContext db, 
            IStudyLogoCreateService studyLogoCreateService,
            IStudyLogoDeleteService studyLogoDeleteService,
            IStudyWbsValidationService studyWbsValidationService,
            IStudyWbsUpdateHandler studyWbsUpdateHandler,
            IStudyEfModelOperationsService studyEfModelOperationsService
          )           
        {
            _db = db;
            _studyLogoCreateService = studyLogoCreateService;
            _studyLogoDeleteService = studyLogoDeleteService;
            _studyWbsValidationService = studyWbsValidationService;

            _studyWbsUpdateHandler = studyWbsUpdateHandler;
            _studyEfModelOperationsService = studyEfModelOperationsService;
        }

        public async Task<Study> UpdateAsync(int studyId, StudyUpdateDto updatedStudy, IFormFile logo = null, CancellationToken cancellationToken = default)
        {
            var spUpdate = Stopwatch.StartNew();

            if (studyId <= 0)
            {
                throw new ArgumentException("Study Id was zero or negative:" + studyId);
            }

            GenericNameValidation.ValidateName(updatedStudy.Name);

            var afterValidation = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            var studyFromDb = await GetStudyAsync(studyId);

            var afterGetStudy = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

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

                await _studyWbsValidationService.ValidateForStudyUpdate(studyFromDb);

                await _studyWbsUpdateHandler.Handle(studyFromDb, cancellationToken);
            }

            var afterWbs = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            if (updatedStudy.DeleteLogo)
            {
                if (!String.IsNullOrWhiteSpace(studyFromDb.LogoUrl))
                {                   
                    await _studyLogoDeleteService.DeleteAsync(studyFromDb.LogoUrl);
                    studyFromDb.LogoUrl = "";
                }
            }
            else if (logo != null)
            {
                studyFromDb.LogoUrl = await _studyLogoCreateService.CreateAsync(studyFromDb, logo);
            }

            var afterLogo = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            studyFromDb.Updated = DateTime.UtcNow;

            EntityValidationUtil.Validate<Study>(studyFromDb);

            var afterValidate = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            await _db.SaveChangesAsync();

            var afterSave = spUpdate.ElapsedMilliseconds;
            spUpdate.Restart();

            return studyFromDb;
        }       

        public async Task<Study> GetStudyAsync(int studyId)
        {
            var queryable = StudyBaseQueries.ActiveStudiesWithParticipantsQueryable(_db);
            return await _studyEfModelOperationsService.GetStudyFromQueryableThrowIfNotFoundOrNoAccess(queryable, studyId, UserOperation.Study_Update_Metadata);
        }
    }
}
