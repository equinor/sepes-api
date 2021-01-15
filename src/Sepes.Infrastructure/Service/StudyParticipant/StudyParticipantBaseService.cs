using AutoMapper;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantBaseService
    {
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;       

        public StudyParticipantBaseService(SepesDbContext db,
            IMapper mapper,
            IUserService userService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;          
        }

   
        protected bool RoleAllreadyExistsForUser(Study study, int userId, string roleName)
        {
            if (study.StudyParticipants == null)
            {
                throw new ArgumentNullException($"Study not loaded properly. Probably missing include for \"StudyParticipants\"");
            }

            if (study.StudyParticipants.Count == 0)
            {
                return false;
            }

            if (study.StudyParticipants.Where(sp => sp.UserId == userId && sp.RoleName == roleName).Any())
            {
                return true;
            }

            return false;
        }

        protected void ValidateRoleNameThrowIfInvalid(string role)
        {
            if ((role.Equals(StudyRoles.SponsorRep) ||
                role.Equals(StudyRoles.StudyOwner) ||
                role.Equals(StudyRoles.StudyViewer) ||
                role.Equals(StudyRoles.VendorAdmin) ||
                role.Equals(StudyRoles.VendorContributor)) == false)
            {
                throw new ArgumentException($"Invalid Role supplied: {role}");
            }
        }
    }
}
