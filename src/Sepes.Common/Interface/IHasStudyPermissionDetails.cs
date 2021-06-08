using System;
using System.Collections.Generic;

namespace Sepes.Common.Interface
{
    public interface IHasStudyPermissionDetails
    {
        public int StudyId { get; set; }

        public bool Restricted { get; set; }

        public Dictionary<int, HashSet<string>> UsersAndRoles { get; set; }
    }
}
