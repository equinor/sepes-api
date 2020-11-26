﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class StudyController : ControllerBase
    {
        readonly IStudyService _studyService;


        public StudyController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync([FromQuery] bool? excludeHidden)
        {
            var studies = await _studyService.GetStudyListAsync(excludeHidden);
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            var study = await _studyService.GetStudyDetailsDtoByIdAsync(studyId, UserOperation.Study_Read);
            return new JsonResult(study);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyAsync(StudyCreateDto newStudy)
        {
            var study = await _studyService.CreateStudyAsync(newStudy);
            return new JsonResult(study);
        }

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            await _studyService.CloseStudyAsync(studyId); //Todo: Switch to correct method
            return new NoContentResult();
        }

        //[HttpDelete("{studyId}")]
        //[Authorize]
        //public async Task<IActionResult> CloseStudyAsync(int studyId)
        //{
        //    await _studyService.CloseStudyAsync(studyId);
        //    return new NoContentResult();
        //}


        [HttpPut("{studyId}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyMetadataAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }


        // For local development, this method requires a running instance of Azure Storage Emulator
        [HttpPut("{studyId}/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLogo(int studyId, [FromForm(Name = "image")] IFormFile studyLogo)
        {
            var updatedStudy = await _studyService.AddLogoAsync(studyId, studyLogo);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/logo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Octet)]
        // For local development, this method requires a running instance of Azure Storage Emulator
        public async Task<IActionResult> GetLogo(int studyId)
        {
            var logoResponse = await _studyService.GetLogoAsync(studyId);

            string fileType = logoResponse.LogoUrl.Split('.').Last();

            if (fileType.Equals("jpg"))
            {
                fileType = "jpeg";
            }

            return File(new System.IO.MemoryStream(logoResponse.LogoBytes), $"image/{fileType}", $"logo_{studyId}.{fileType}");
        }
    }
}
