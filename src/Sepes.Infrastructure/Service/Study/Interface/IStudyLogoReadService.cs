using Microsoft.AspNetCore.Http;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyLogoReadService
    {
        Task DecorateLogoUrlsWithSAS(IEnumerable<IHasLogoUrl> studyDtos);

        Task DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo);         
    }
}