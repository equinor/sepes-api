using AutoMapper;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class LookupService : ILookupService
    {
        public SepesDbContext _db;
        public IMapper _mapper;
        public IUserService _userService;
        public IStudyModelService _studyModelService;

        public LookupService(SepesDbContext db, IMapper mapper, IUserService userService, IStudyModelService studyModelService)
        {
            _mapper = mapper;
            _userService = userService;
            _db = db;
            _studyModelService = studyModelService;
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

        public async Task<IEnumerable<LookupDto>> StudyRolesUserCanGive(int studyId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var studyFromDb = await _studyModelService.GetWitParticipantsNoAccessCheck(studyId);
                    
            var existingParticipantRoles = studyFromDb.StudyParticipants.Where(x => x.UserId == user.Id).ToList();
            var result = new Dictionary<string, LookupDto>();

            foreach (var curExistingRole in existingParticipantRoles)
            {
                if (result.ContainsKey(curExistingRole.RoleName))
                {
                    continue;
                }
             
                if (curExistingRole.RoleName == Constants.StudyRoles.VendorContributor)
                {
                    EnsureItemExistsForRole(result, Constants.StudyRoles.VendorContributor);
                }
                else if (curExistingRole.RoleName == Constants.StudyRoles.VendorAdmin)
                {
                    EnsureItemExistsForRole(result, Constants.StudyRoles.VendorAdmin);
                    EnsureItemExistsForRole(result, Constants.StudyRoles.VendorContributor);
                }
                else if (curExistingRole.RoleName == Constants.StudyRoles.SponsorRep || curExistingRole.RoleName == Constants.StudyRoles.StudyOwner)
                {
                    EnsureItemExistsForRole(result, Constants.StudyRoles.SponsorRep);
                    EnsureItemExistsForRole(result, Constants.StudyRoles.VendorAdmin);
                    EnsureItemExistsForRole(result, Constants.StudyRoles.VendorContributor);
                    EnsureItemExistsForRole(result, Constants.StudyRoles.StudyViewer);
                }               
            }

            return result.Values.ToList();
        }

        static void EnsureItemExistsForRole(Dictionary<string, LookupDto> lookupValues, string key)
        {
            if (!lookupValues.ContainsKey(key))
            {
                lookupValues.Add(key, new LookupDto(key));
            }
        }
    }
}
