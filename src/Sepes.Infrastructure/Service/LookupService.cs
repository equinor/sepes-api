using AutoMapper;
using Sepes.Common.Dto;
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
        public IStudyEfModelService _studyModelService;

        public LookupService(SepesDbContext db, IMapper mapper, IUserService userService, IStudyEfModelService studyModelService)
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
                new LookupDto{ Key= Common.Constants.StudyRoles.SponsorRep, DisplayValue = Common.Constants.StudyRoles.SponsorRep },
                new LookupDto{ Key= Common.Constants.StudyRoles.VendorAdmin,  DisplayValue = Common.Constants.StudyRoles.VendorAdmin },
                new LookupDto{ Key= Common.Constants.StudyRoles.VendorContributor,  DisplayValue = Common.Constants.StudyRoles.VendorContributor },
                new LookupDto{ Key= Common.Constants.StudyRoles.StudyViewer,  DisplayValue =  Common.Constants.StudyRoles.StudyViewer }
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
             
                if (curExistingRole.RoleName == Common.Constants.StudyRoles.VendorContributor)
                {
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.VendorContributor);
                }
                else if (curExistingRole.RoleName == Common.Constants.StudyRoles.VendorAdmin)
                {
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.VendorAdmin);
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.VendorContributor);
                }
                else if (curExistingRole.RoleName == Common.Constants.StudyRoles.SponsorRep || curExistingRole.RoleName == Common.Constants.StudyRoles.StudyOwner)
                {
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.SponsorRep);
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.VendorAdmin);
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.VendorContributor);
                    EnsureItemExistsForRole(result, Common.Constants.StudyRoles.StudyViewer);
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
