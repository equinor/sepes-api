﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyController : ControllerBase
    {
        readonly IStudyService _studyService;

        public StudyController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        //Get list of studies
        [HttpGet]
        public async Task<IActionResult> GetStudies([FromQuery] bool? includeRestricted)
        {
            var studies = await _studyService.GetStudiesAsync(includeRestricted);
            return new JsonResult(studies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudy(int id)
        {
            var study = await _studyService.GetStudyByIdAsync(id);
            return new JsonResult(study);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateStudy(StudyDto newStudy)
        {
            var study = await _studyService.CreateStudyAsync(newStudy);
            return new JsonResult(study);
        }

        //PUT localhost:8080/api/studies/1/details
        [HttpPut("{id}/details")]
        public async Task<IActionResult> UpdateStudyDetails(int id, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyDetailsAsync(id, study);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{id}/datasets/{dataSetId}")]
        public async Task<IActionResult> AddDataSet(int id, int dataSetId)
        {
            var updatedStudy = await _studyService.AddDataset(id, dataSetId);
            return new JsonResult(updatedStudy);
        }

        //TODO:FIX
        // Should this be addDataset or AddCustomDataSet?
        [HttpPut("{id}/datasets/studyspecific")]
        public async Task<IActionResult> AddDataSet(int id, int datasetId, StudySpecificDatasetDto newDataset)
        {
            //TODO: Perform checks on dataset?
            //TODO: Post custom dataset
            

            var updatedStudy = await _studyService.AddCustomDataset(id, datasetId, newDataset);
            return new JsonResult(updatedStudy);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudy(int id, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyAsync(id, study);
            return new JsonResult(updatedStudy);
        }


        //[HttpPost]
        //public async Task<ActionResult<StudyInputDto>> SaveStudy([FromBody] StudyInputDto[] studies)
        //{
        //    //Studies [1] is what the frontend claims the changes is based on while Studies [0] is the new version

        //    //If based is null it can be assumed this will be a newly created study
        //    if (studies[1] == null)
        //    {
        //        StudyDto study = await _studyService.Save(studies[0].ToStudy(), null);
        //        return study.ToStudyInput();
        //    }
        //    //Otherwise it must be a change.
        //    else
        //    {
        //        StudyDto study = await _studyService.Save(studies[0].ToStudy(), studies[1].ToStudy());
        //        return study.ToStudyInput();
        //    }
        //}



        //[HttpGet("archived")]
        //public IEnumerable<StudyInputDto> GetArchived()
        //{
        //    return _studyService.GetStudies(new UserDto("", "", ""), true).Select(study => study.ToStudyInput());
        //}

        //[HttpGet("dataset")]
        //public async Task<string> GetDataset()
        //{
        //    return await _sepesDb.getDatasetList();
        //}
    }

}
