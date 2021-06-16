using Microsoft.Extensions.Logging;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyWbsValidationService : IStudyWbsValidationService
    {
        readonly ILogger _logger;      
        readonly IWbsValidationService _wbsValidationService;

        public StudyWbsValidationService(ILogger<StudyWbsValidationService> logger, IWbsValidationService wbsValidationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));         
            _wbsValidationService = wbsValidationService ?? throw new ArgumentNullException(nameof(wbsValidationService));
        }     

        public async Task ValidateForStudyCreate(Study study)
        {
            try
            {
                await PerformValidationAndUpdateStudy(study);               
            }
            catch (Exception ex)
            {
                _logger.LogError("Wbs validation failed", ex);              
            }           
        }

        public async Task ValidateForStudyUpdate(Study study, bool hasSandboxOrDataset)
        { 
            try
            {
                await PerformValidationAndUpdateStudy(study);               
            }
            catch (Exception ex)
            {
                _logger.LogError("Wbs validation failed", ex);
            }

            if (!study.WbsCodeValid && hasSandboxOrDataset)
            {
                throw new InvalidWbsException($"Invalid WBS Code {study.WbsCode} for Study {study.Id}",
                    $"Invalid WBS code for Study. Study has active Datasets and/or Sandboxes. You cannot change to an invalid WBS code.");
            }
        }

        public async Task ValidateForSandboxCreationOrThrow(Study study)
        {
            await ValidateForDependentEntityCreationOrThrow(study, "Sandbox");
        }

        public async Task ValidateForDatasetCreationOrThrow(Study study)
        {
            await ValidateForDependentEntityCreationOrThrow(study, "Dataset");
        }

        async Task ValidateForDependentEntityCreationOrThrow(Study study, string entityName)
        {
            if (String.IsNullOrWhiteSpace(study.WbsCode))
            {
                throw new InvalidWbsException($"Empty WBS Code for Study {study.Id}", $"Missing WBS code for Study. Study requires WBS code before {entityName} can be created.");
            }

            if (study.WbsCodeValid)
            {
                return;
            }

            await PerformValidationAndUpdateStudy(study);

            if (!study.WbsCodeValid)
            {
                throw new InvalidWbsException($"Invalid WBS Code {study.WbsCode} for Study {study.Id}", $"Invalid WBS code for Study. Study requires valid WBS code before {entityName} can be created.");
            }
        }        

        async Task PerformValidationAndUpdateStudy(Study study)
        {
            bool isValid;

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
            }
        }
    }
}