using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyLogoReadService : IStudyLogoReadService
    {
        readonly string _containerName = "studylogos";

        readonly ILogger _logger;      
    
        readonly IAzureBlobStorageService _azureBlobStorageService;
        readonly IAzureBlobStorageUriBuilderService _azureStorageAccountTokenService;

        public StudyLogoReadService(ILogger<StudyLogoReadService> logger,         
            IAzureBlobStorageService blobService,
            IAzureBlobStorageUriBuilderService azureStorageAccountTokenService)
        {
            _logger = logger;  
            _azureBlobStorageService = blobService;
            _azureStorageAccountTokenService = azureStorageAccountTokenService;

            _azureBlobStorageService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
            _azureStorageAccountTokenService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
        }

        public async Task DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo)
        {
            try
            {
                var uriBuilder = await CreateFileDownloadUriBuilder(_containerName);

                if (uriBuilder == null)
                {
                    _logger.LogError("Unable to decorate logo urls");
                }
                else
                {
                    DecorateLogoUrlWithSAS(uriBuilder, hasLogo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to decorate Study item with logo url. Se exception info");
            }
        }

        public async Task DecorateLogoUrlsWithSAS(IEnumerable<IHasLogoUrl> studyDtos)
        {
            try
            {
                var uriBuilder = await CreateFileDownloadUriBuilder(_containerName);

                if (uriBuilder == null)
                {
                    _logger.LogError("Unable to decorate logo urls");
                }
                else
                {
                    foreach (var curDto in studyDtos)
                    {
                        DecorateLogoUrlWithSAS(uriBuilder, curDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to decorate list of Studies with logo urls. Se exception info");
            }
        }

        async Task<UriBuilder> CreateFileDownloadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureStorageAccountTokenService.CreateUriBuilder(containerName, cancellationToken: cancellationToken);
        }

        void DecorateLogoUrlWithSAS(UriBuilder uriBuilder, IHasLogoUrl hasLogo)
        {
            if (!String.IsNullOrWhiteSpace(hasLogo.LogoUrl))
            {
                uriBuilder.Path = string.Format("{0}/{1}", _containerName, hasLogo.LogoUrl);
                hasLogo.LogoUrl = uriBuilder.Uri.ToString();
            }
            else
            {
                hasLogo.LogoUrl = null;
            }
        } 
    }
}
