using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using Sepes.Infrastructure.Constants;
using AutoMapper;

namespace Sepes.Infrastructure.Service
{
    public class LookupService : ILookupService
    {
        public IMapper _mapper;

        public LookupService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<LookupDto> AzureRegions()
        {
            var availableRegions = AzureRegionConstants.Regions;

            var mappedRegions = _mapper.Map<IEnumerable<LookupDto>>(availableRegions);

            return mappedRegions;         
        }

        public IEnumerable<LookupDto> StudyRoles()
        {
            return new List<LookupDto>()
            {
                new LookupDto{ Key= Constants.StudyRoles.SponsorRep, DisplayValue = Constants.StudyRoles.SponsorRep },
                new LookupDto{ Key= Constants.StudyRoles.VendorAdmin,  DisplayValue = Constants.StudyRoles.VendorAdmin },
                new LookupDto{ Key= Constants.StudyRoles.VendorContributor,  DisplayValue = Constants.StudyRoles.VendorContributor },
                new LookupDto{ Key= Constants.StudyRoles.StudyViewer,  DisplayValue =  Constants.StudyRoles.StudyViewer }
            };
        }
    }
}
