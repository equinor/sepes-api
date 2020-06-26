using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Graph.RBAC.Fluent.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public StudyController(IStudyService studyService, IConfiguration configuration)
        {
            _studyService = studyService;
            _configuration = configuration;
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
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> CreateStudy(StudyDto newStudy)
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

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudy(int id, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyAsync(id, study);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudy(int id)
        {
            var study = await _studyService.DeleteStudyAsync(id);
            return new JsonResult(study);
        }

        //PUT localhost:8080/api/studies/1/details
        [HttpPut("{id}/details")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateStudyDetails(int id, StudyDto study)
        {
            var updatedStudy = await _studyService.UpdateStudyDetailsAsync(id, study);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{id}/sandboxes")]
        public async Task<IActionResult> GetSandboxesByStudyIdAsync(int id)
        {
            var sandboxes = await _studyService.GetSandboxesByStudyIdAsync(id);
            return new JsonResult(sandboxes);
        }

        [HttpPut("{id}/sandboxes")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddSandbox(int id, SandboxDto newSandbox)
        {
            var updatedStudy = await _studyService.AddSandboxAsync(id, newSandbox);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{id}/sandboxes/{sandboxId}")]
        public async Task<IActionResult> RemoveSandbox(int id, int sandboxId)
        {
            var updatedStudy = await _studyService.RemoveSandboxAsync(id, sandboxId);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{id}/datasets/{datasetId}")]
        public async Task<IActionResult> AddDataSet(int id, int datasetId)
        {
            var updatedStudy = await _studyService.AddDatasetAsync(id, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{id}/datasets/{datasetId}")]
        public async Task<IActionResult> RemoveDataSet(int id, int datasetId)
        {
            var updatedStudy = await _studyService.RemoveDatasetAsync(id, datasetId);
            return new JsonResult(updatedStudy);
        }

        //TODO:FIX
        // Should this be addDataset or AddCustomDataSet?
        [HttpPut("{id}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> AddDataSet(int id, int datasetId, StudySpecificDatasetDto newDataset)
        {
            //TODO: Perform checks on dataset?
            //TODO: Post custom dataset
            var updatedStudy = await _studyService.AddCustomDatasetAsync(id, datasetId, newDataset);
            return new JsonResult(updatedStudy);
        }

        [HttpPut("{id}/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLogo(int id, IFormFile studyLogo)
        {
            string storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];
            var updatedStudy = await _studyService.AddLogoAsync(id, studyLogo, storageConnectionString);
            return new JsonResult(updatedStudy);
        }

        [HttpGet("{id}/logo")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Octet)]
        //[Produces("img/png")]
        public async Task<IActionResult> GetLogo(int id)
        {
            string storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];
            byte[] logo = await _studyService.GetLogoAsync(id, storageConnectionString);
            var studyDtoFromDb = await _studyService.GetStudyByIdAsync(id);
            string fileType = studyDtoFromDb.LogoUrl.Split('.').Last();
            if (fileType.Equals("jpg"))
            {
                fileType = "jpeg";
            }
            return File(new System.IO.MemoryStream(logo), $"image/{fileType}", $"logo_{id}.{fileType}");
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
