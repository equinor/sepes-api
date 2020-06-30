using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Graph.RBAC.Fluent.Models;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
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

        //Get list of studies
        [HttpGet]
        public async Task<IActionResult> GetStudiesAsync([FromQuery] bool? includeRestricted)
        {
            var studies = await _studyService.GetStudiesAsync(includeRestricted);
            return new JsonResult(studies);
        }

        [HttpGet("{studyId}")]
        public async Task<IActionResult> GetStudyAsync(int studyId)
        {
            var study = await _studyService.GetStudyByIdAsync(studyId);
            return new JsonResult(study);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateStudyAsync(StudyDto newStudy)
        {
            var study = await _studyService.CreateStudyAsync(newStudy);
            return new JsonResult(study);
        }

      //[HttpPost()]
      //[Consumes(MediaTypeNames.Application.Json, "multipart/form-data")]
      //public async Task<IActionResult> CreateStudyWithPicture(StudyDto newStudy, IFormFile studyLogo)
      //{
      //    var study = await _studyService.CreateStudyAsync(newStudy, studyLogo);
      //    return new JsonResult(study);
      //}

        [HttpPut("{studyId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudyAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}")]
        public async Task<IActionResult> DeleteStudyAsync(int studyId)
        {
            var study = await _studyService.DeleteStudyAsync(studyId);
            return new JsonResult(study);
        }

        //PUT localhost:8080/api/studies/1/details
        [HttpPut("{studyId}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudyDetailsAsync(int studyId, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyDetailsAsync(studyId, study);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{studyId}/sandboxes")]
        public async Task<IActionResult> GetSandboxesByStudyIdAsync(int studyId)
        {
            var sandboxes = await _studyService.GetSandboxesByStudyIdAsync(studyId);
            return new JsonResult(sandboxes);
        }

        [HttpPut("{studyId}/sandboxes")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddSandboxAsync(int studyId, SandboxDto newSandbox)
        {
            var updatedStudy = await _studyService.AddSandboxAsync(studyId, newSandbox);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/sandboxes/{sandboxId}")]
        public async Task<IActionResult> RemoveSandboxAsync(int studyId, int sandboxId)
        {
            var updatedStudy = await _studyService.RemoveSandboxAsync(studyId, sandboxId);
            return new JsonResult(updatedStudy);
        }     

        [HttpPut("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> AddDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyService.AddDatasetAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/datasets/{datasetId}")]
        public async Task<IActionResult> RemoveDataSetAsync(int studyId, int datasetId)
        {
            var updatedStudy = await _studyService.RemoveDatasetAsync(studyId, datasetId);
            return new JsonResult(updatedStudy);
        }

        //TODO:FIX
        // Should this be addDataset or AddCustomDataSet?
        [HttpPut("{studyId}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddDataSetAsync(int studyId, int datasetId, StudySpecificDatasetDto newDataset)
        {
            //TODO: Perform checks on dataset?
            //TODO: Post custom dataset
            var updatedStudy = await _studyService.AddCustomDatasetAsync(studyId, datasetId, newDataset);
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
            byte[] logo = await _studyService.GetLogoAsync(studyId);
            var studyDtoFromDb = await _studyService.GetStudyByIdAsync(studyId);
            string fileType = studyDtoFromDb.LogoUrl.Split('.').Last();
            if (fileType.Equals("jpg"))
            {
                fileType = "jpeg";
            }
            return File(new System.IO.MemoryStream(logo), $"image/{fileType}", $"logo_{studyId}.{fileType}");
            //return new ObjectResult(logo);
        }

        //[HttpPost]
        //[Authorize]
        //public async Task<ActionResult<StudyLogo>> AddStudyLogo(StudyLogo studyLogo)
        //{
        //    //var blob = new UploadToBlobStorage();
        //    //blob.UploadBlob(piece.ImageBlob, Configuration["ConnectionStrings:BlobConnection"]);
        //    return CreatedAtAction("GetPiece", new { id = piece.Id }, piece);
        //}

        //// POST: api/Pieces/Blob
        //[HttpPost("Blob")]
        //[Authorize]
        ////[Consumes("multipart/form-data")]
        //public string PostBlob([FromForm(Name = "image")] IFormFile image)
        //{
        //    var uploadToBlobStorage = new UploadToBlobStorage();
        //    var fileName = uploadToBlobStorage.UploadBlob(image, Configuration["ConnectionStrings:BlobConnection"]);
        //    return fileName;
        //}



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

        [HttpPut("{studyId}/participants/{participantId}/{role}")]
        public async Task<IActionResult> AddParticipantAsync(int studyId, int participantId, string role)
        {
            var updatedStudy = await _studyService.AddParticipantAsync(studyId, participantId, role);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{studyId}/participants/{participantId}")]
        public async Task<IActionResult> RemoveParticipantAsync(int studyId, int participantId)
        {
            var updatedStudy = await _studyService.RemoveParticipantAsync(studyId, participantId);
            return new JsonResult(updatedStudy);
        }       
    }

}
