using AutoMapper;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Sepes.Infrastructure.Service.DataModelService.Interface;

namespace Sepes.Infrastructure.Service
{
    public class LookupService : ILookupService
    {
        public IMapper _mapper;
        public IUserService _userService;
        public SepesDbContext _db;
        public IStudyModelService _studyModelService;

        public LookupService(IMapper mapper, IUserService userService, SepesDbContext db, IStudyModelService studyModelService)
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
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Read, true);
            var user = await _userService.GetCurrentUserAsync();
            //var studiesAccessWherePart = StudyAccessQueryBuilder.CreateAccessWhereClause(user, UserOperation.Study_Read);
            var userRoles = studyFromDb.StudyParticipants.Where(x => x.UserId == user.Id).ToList();
            var returnList = new List<LookupDto>();
            foreach (var role in userRoles)
            {
                var list = returnList.Where(x => x.Key == role.RoleName).ToArray();
                if (list.Length > 0)
                {
                    continue;
                }
                if (role.RoleName == Constants.StudyRoles.StudyViewer)
                {
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.StudyViewer, DisplayValue = Constants.StudyRoles.StudyViewer });
                }
                else if (role.RoleName == Constants.StudyRoles.VendorContributor)
                {
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.StudyViewer, DisplayValue = Constants.StudyRoles.StudyViewer });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.VendorContributor, DisplayValue = Constants.StudyRoles.VendorContributor });
                }
                else if (role.RoleName == Constants.StudyRoles.VendorAdmin)
                {
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.StudyViewer, DisplayValue = Constants.StudyRoles.StudyViewer });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.VendorAdmin, DisplayValue = Constants.StudyRoles.VendorAdmin });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.VendorContributor, DisplayValue = Constants.StudyRoles.VendorContributor });
                }
                else if (role.RoleName == Constants.StudyRoles.SponsorRep)
                {
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.StudyViewer, DisplayValue = Constants.StudyRoles.StudyViewer });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.SponsorRep, DisplayValue = Constants.StudyRoles.SponsorRep });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.VendorAdmin, DisplayValue = Constants.StudyRoles.VendorAdmin });
                    returnList.Add(new LookupDto { Key = Constants.StudyRoles.VendorContributor, DisplayValue = Constants.StudyRoles.VendorContributor });
                }
            }

            return returnList.ToArray();
        }
    }
}
