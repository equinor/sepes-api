using AutoMapper;
using Sepes.Common.Dto.Study;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Automapper
{
    public class StudyToStudyPermissionDetailsResolver : IValueResolver<Study, IHasStudyPermissionDetails, Dictionary<int, HashSet<string>>>
    { 
        public StudyToStudyPermissionDetailsResolver()
        {
           
        }

        public Dictionary<int, HashSet<string>> Resolve(Study source, IHasStudyPermissionDetails destination, Dictionary<int, HashSet<string>> destMember, ResolutionContext context)
        {
            if(source.StudyParticipants == null)
            {
                throw new ArgumentNullException("source");
            }

            var participants = source.StudyParticipants.GroupBy(sp=> sp.UserId).ToDictionary(spGrp => spGrp.Key, spGrp => new HashSet<string>(spGrp.Select(sp=> sp.RoleName).ToList()) );

            var hashSet = new Dictionary<int, HashSet<string>>(participants);

            return hashSet;        
        }
    }

    public class StudyDetailsToStudyPermissionDetailsResolver : IValueResolver<StudyDetailsDto, IHasStudyPermissionDetails, Dictionary<int, HashSet<string>>>
    {
        public StudyDetailsToStudyPermissionDetailsResolver()
        {

        }

        public Dictionary<int, HashSet<string>> Resolve(StudyDetailsDto source, IHasStudyPermissionDetails destination, Dictionary<int, HashSet<string>> destMember, ResolutionContext context)
        {
            if (source.Participants == null)
            {
                throw new ArgumentNullException("source");
            }

            var participants = source.Participants.GroupBy(sp => sp.UserId).ToDictionary(spGrp => spGrp.Key, spGrp => new HashSet<string>(spGrp.Select(sp => sp.Role).ToList()));

            var hashSet = new Dictionary<int, HashSet<string>>(participants);

            return hashSet;
        }
    }
}
