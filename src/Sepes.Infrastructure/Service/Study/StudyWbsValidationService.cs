using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Infrastructure.Service
{
    public class StudyWbsValidationService : IStudyWbsValidationService
    {
        readonly ILogger _logger;
        readonly IWbsValidationService _wbsValidationService;

        public StudyWbsValidationService(ILogger<StudyWbsValidationService> logger, IWbsValidationService wbsValidationService)
        {
            _logger = logger;
            _wbsValidationService =
                wbsValidationService ?? throw new ArgumentNullException(nameof(wbsValidationService));
        }

        public async Task ValidateForStudyCreateOrUpdate(Study study)
        {
            try
            {
                await PerformValidation(study, false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Wbs validation failed", ex);              
            }           
        }

        public async Task ValidateForSandboxCreationOrThrow(Study study)
        {
            if (String.IsNullOrWhiteSpace(study.WbsCode))
            {
                throw new InvalidWbsException($"Empty WBS Code for Study {study.Id}","Missing WBS code for Study. Study requires WBS code before Sandbox can be created.");
            }
            
            if (study.WbsCodeValid)
            {
                return;
            }

            await PerformValidation(study, true);
        }
        
        async Task PerformValidation(Study study, bool throwIfInvalid)
        {
            bool isValid = false;

            try
            {
                isValid = await _wbsValidationService.IsValid(study.WbsCode);
            }
            catch (Exception ex)
            {
                throw new InvalidWbsException($"WBS validation failed for Study {study.Id}, code: {study.WbsCode}", "Wbs validation failed.", ex);               
            }

            if (isValid)
            {
                study.WbsCodeValid = true;
                study.WbsCodeValidatedAt = DateTime.UtcNow;
            }
            else
            {
                study.WbsCodeValid = false;
                study.WbsCodeValidatedAt = default;

                if (throwIfInvalid)
                {
                    throw new InvalidWbsException($"Invalid WBS Code {study.WbsCode} for Study {study.Id}","Invalid WBS code for Study. Study requires valid WBS code before Sandbox can be created.");
                }
            }
        }
    }
}